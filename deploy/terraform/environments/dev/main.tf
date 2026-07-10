terraform {
  required_version = ">= 1.5.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 6.0"
    }
  }

  backend "local" {}
}

provider "aws" {
  region = var.aws_region
}

locals {
  name_prefix = "${var.project_name}-${var.environment}"

  common_tags = {
    Project     = var.project_name
    Environment = var.environment
    ManagedBy   = "Terraform"
  }

  db_connection_string = "Host=${module.aurora.cluster_endpoint};Port=${module.aurora.cluster_port};Database=${var.db_name};Username=${var.db_username};Password=${var.db_password}"
  redis_connection     = "${module.redis.primary_endpoint_address}:${module.redis.port}"
}

data "aws_iam_policy_document" "ecs_tasks_assume_role" {
  statement {
    effect = "Allow"

    principals {
      type        = "Service"
      identifiers = ["ecs-tasks.amazonaws.com"]
    }

    actions = ["sts:AssumeRole"]
  }
}

resource "aws_iam_role" "ecs_task_execution" {
  name               = "${local.name_prefix}-ecs-execution-role"
  assume_role_policy = data.aws_iam_policy_document.ecs_tasks_assume_role.json
  tags               = local.common_tags
}

resource "aws_iam_role_policy_attachment" "ecs_task_execution" {
  role       = aws_iam_role.ecs_task_execution.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

data "aws_iam_policy_document" "ecs_task_execution_secrets" {
  statement {
    effect = "Allow"
    actions = [
      "secretsmanager:GetSecretValue"
    ]
    resources = [
      aws_secretsmanager_secret.db_connection.arn,
      aws_secretsmanager_secret.jwt_key.arn
    ]
  }
}

resource "aws_iam_role_policy" "ecs_task_execution_secrets" {
  name   = "${local.name_prefix}-ecs-execution-secrets"
  role   = aws_iam_role.ecs_task_execution.id
  policy = data.aws_iam_policy_document.ecs_task_execution_secrets.json
}

resource "aws_iam_role" "ecs_task" {
  name               = "${local.name_prefix}-ecs-task-role"
  assume_role_policy = data.aws_iam_policy_document.ecs_tasks_assume_role.json
  tags               = local.common_tags
}

data "aws_iam_policy_document" "ecs_task_s3" {
  statement {
    effect = "Allow"
    actions = [
      "s3:GetObject",
      "s3:PutObject",
      "s3:DeleteObject",
      "s3:AbortMultipartUpload",
      "s3:ListBucket",
      "s3:GetBucketLocation"
    ]
    resources = [
      module.attachments_bucket.bucket_arn,
      "${module.attachments_bucket.bucket_arn}/*"
    ]
  }
}

resource "aws_iam_role_policy" "ecs_task_s3" {
  name   = "${local.name_prefix}-attachments-access"
  role   = aws_iam_role.ecs_task.id
  policy = data.aws_iam_policy_document.ecs_task_s3.json
}

resource "aws_secretsmanager_secret" "db_connection" {
  name                    = "${local.name_prefix}/api/db-connection"
  recovery_window_in_days = 0
  tags                    = local.common_tags
}

resource "aws_secretsmanager_secret_version" "db_connection" {
  secret_id     = aws_secretsmanager_secret.db_connection.id
  secret_string = local.db_connection_string
}

resource "aws_secretsmanager_secret" "jwt_key" {
  name                    = "${local.name_prefix}/api/jwt-key"
  recovery_window_in_days = 0
  tags                    = local.common_tags
}

resource "aws_secretsmanager_secret_version" "jwt_key" {
  secret_id     = aws_secretsmanager_secret.jwt_key.id
  secret_string = var.jwt_key
}

module "network" {
  source = "../../modules/network"

  cidr_block      = var.vpc_cidr
  public_subnets  = var.public_subnets
  private_subnets = var.private_subnets
  tags            = local.common_tags
}

resource "aws_security_group" "alb" {
  name        = "${local.name_prefix}-alb-sg"
  description = "ALB security group for ${local.name_prefix}."
  vpc_id      = module.network.vpc_id

  ingress {
    description = "HTTP from the internet"
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    description = "All outbound traffic"
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = local.common_tags
}

resource "aws_security_group" "ecs_service" {
  name        = "${local.name_prefix}-ecs-sg"
  description = "ECS service security group for ${local.name_prefix}."
  vpc_id      = module.network.vpc_id

  ingress {
    description     = "Application traffic from the ALB"
    from_port       = var.container_port
    to_port         = var.container_port
    protocol        = "tcp"
    security_groups = [aws_security_group.alb.id]
  }

  egress {
    description = "All outbound traffic"
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = local.common_tags
}

module "ecs_cluster" {
  source = "../../modules/ecs_cluster"

  name = "${local.name_prefix}-cluster"
}

module "alb" {
  source = "../../modules/alb"

  name              = substr("${local.name_prefix}-alb", 0, 32)
  subnet_ids        = module.network.public_subnet_ids
  security_group_id = aws_security_group.alb.id
  tags              = local.common_tags
}

module "attachments_bucket" {
  source = "../../modules/s3_bucket"

  bucket_name       = var.attachments_bucket_name
  allowed_role_arns = [aws_iam_role.ecs_task.arn]
  lifecycle_rules   = var.attachments_lifecycle_rules
  cors_rules        = var.attachments_cors_rules
  tags              = local.common_tags
}

module "aurora" {
  source = "../../modules/aurora_postgres"

  cluster_identifier      = "${local.name_prefix}-aurora"
  vpc_id                  = module.network.vpc_id
  subnet_ids              = module.network.private_subnet_ids
  database_name           = var.db_name
  master_username         = var.db_username
  master_password         = var.db_password
  engine_version          = var.aurora_engine_version
  instance_count          = var.aurora_instance_count
  serverlessv2_min_capacity = var.aurora_serverlessv2_min_capacity
  serverlessv2_max_capacity = var.aurora_serverlessv2_max_capacity
  backup_retention_period = var.aurora_backup_retention_period
  deletion_protection     = false
  skip_final_snapshot     = true
  apply_immediately       = true
  allowed_security_groups = [
    {
      security_group_id = aws_security_group.ecs_service.id
      description       = "Allow PostgreSQL from ECS tasks."
    }
  ]
  tags = local.common_tags
}

module "redis" {
  source = "../../modules/elasticache_redis"

  replication_group_id     = "${local.name_prefix}-redis"
  description              = "Redis cache for ${local.name_prefix}."
  vpc_id                   = module.network.vpc_id
  subnet_ids               = module.network.private_subnet_ids
  node_type                = var.redis_node_type
  engine_version           = var.redis_engine_version
  num_cache_clusters       = var.redis_num_cache_clusters
  automatic_failover_enabled = var.redis_automatic_failover_enabled
  multi_az_enabled         = var.redis_multi_az_enabled
  transit_encryption_enabled = true
  at_rest_encryption_enabled = true
  auth_token               = var.redis_auth_token
  apply_immediately        = true
  snapshot_retention_limit = var.redis_snapshot_retention_limit
  allowed_security_groups = [
    {
      security_group_id = aws_security_group.ecs_service.id
      description       = "Allow Redis from ECS tasks."
    }
  ]
  tags = local.common_tags
}

module "ecs_task" {
  source = "../../modules/ecs_task"

  family             = "${local.name_prefix}-api"
  container_name     = var.container_name
  image              = var.container_image
  container_port     = var.container_port
  cpu                = var.task_cpu
  memory             = var.task_memory
  execution_role_arn = aws_iam_role.ecs_task_execution.arn
  task_role_arn      = aws_iam_role.ecs_task.arn
  aws_region         = var.aws_region
  environment = {
    ASPNETCORE_ENVIRONMENT           = "Development"
    ASPNETCORE_URLS                  = "http://+:${var.container_port}"
    CachingOptions__Redis            = local.redis_connection
    CachingOptions__EnableSsl        = "true"
    JwtOptions__Issuer               = var.jwt_issuer
    JwtOptions__Audience             = var.jwt_audience
    JwtOptions__DurationInMinutes    = tostring(var.jwt_duration_minutes)
    JwtOptions__RefreshTokenDurationInDays = tostring(var.jwt_refresh_token_duration_days)
    AllowedHosts                     = "*"
  }
  secrets = {
    ConnectionStrings__DefaultConnection = aws_secretsmanager_secret.db_connection.arn
    JwtOptions__Key                      = aws_secretsmanager_secret.jwt_key.arn
  }
  tags = local.common_tags
}

module "ecs_service" {
  source = "../../modules/ecs_service"

  name                = "${local.name_prefix}-api"
  cluster_arn         = module.ecs_cluster.arn
  cluster_name        = module.ecs_cluster.name
  task_definition_arn = module.ecs_task.task_definition_arn
  desired_count       = var.service_desired_count
  subnet_ids          = module.network.public_subnet_ids
  security_group_ids  = [aws_security_group.ecs_service.id]
  assign_public_ip    = true

  container_name   = module.ecs_task.container_name
  container_port   = module.ecs_task.container_port
  vpc_id           = module.network.vpc_id
  listener_arn     = module.alb.listener_arn
  create_listener_rule = true
  listener_rule_priority = var.listener_rule_priority
  listener_rule_path_patterns = ["/*"]
  health_check_path = "/health/ready"

  enable_autoscaling       = true
  autoscaling_min_capacity = var.autoscaling_min_capacity
  autoscaling_max_capacity = var.autoscaling_max_capacity
  memory_target_value      = var.memory_target_value
  scale_in_cooldown        = var.scale_in_cooldown
  scale_out_cooldown       = var.scale_out_cooldown

  tags = local.common_tags
}
