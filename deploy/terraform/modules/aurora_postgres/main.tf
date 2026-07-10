data "aws_region" "current" {}

locals {
  tags = merge(
    {
      Name = var.cluster_identifier
    },
    var.tags
  )

  security_group_ids = concat(
    var.create_security_group ? [aws_security_group.this[0].id] : [],
    var.additional_security_group_ids
  )

  security_group_rules_by_sg = {
    for rule in var.allowed_security_groups :
    rule.security_group_id => rule
  }

  security_group_rules_by_cidr = {
    for index, rule in var.allowed_cidr_blocks :
    tostring(index) => rule
  }
}

resource "aws_db_subnet_group" "this" {
  name       = var.db_subnet_group_name != null ? var.db_subnet_group_name : "${var.cluster_identifier}-subnet-group"
  subnet_ids = var.subnet_ids
  tags       = local.tags
}

resource "aws_security_group" "this" {
  count       = var.create_security_group ? 1 : 0
  name        = "${var.cluster_identifier}-sg"
  description = "Security group for ${var.cluster_identifier} Aurora PostgreSQL cluster."
  vpc_id      = var.vpc_id
  tags        = local.tags
}

resource "aws_vpc_security_group_ingress_rule" "from_security_groups" {
  for_each = var.create_security_group ? local.security_group_rules_by_sg : {}

  security_group_id            = aws_security_group.this[0].id
  referenced_security_group_id = each.value.security_group_id
  ip_protocol                  = "tcp"
  from_port                    = var.port
  to_port                      = var.port
  description                  = each.value.description
}

resource "aws_vpc_security_group_ingress_rule" "from_cidrs" {
  for_each = var.create_security_group ? local.security_group_rules_by_cidr : {}

  security_group_id = aws_security_group.this[0].id
  cidr_ipv4         = each.value.cidr_block
  ip_protocol       = "tcp"
  from_port         = var.port
  to_port           = var.port
  description       = each.value.description
}

resource "aws_vpc_security_group_egress_rule" "all" {
  count = var.create_security_group ? 1 : 0

  security_group_id = aws_security_group.this[0].id
  cidr_ipv4         = "0.0.0.0/0"
  ip_protocol       = "-1"
  description       = "Allow outbound traffic."
}

resource "aws_cloudwatch_log_group" "postgresql" {
  count = length(var.enabled_cloudwatch_logs_exports) > 0 && var.create_cloudwatch_log_group ? 1 : 0

  name              = "/aws/rds/cluster/${var.cluster_identifier}/postgresql"
  retention_in_days = var.cloudwatch_log_group_retention_in_days
  kms_key_id        = var.cloudwatch_log_group_kms_key_id
  tags              = local.tags
}

resource "aws_rds_cluster" "this" {
  cluster_identifier = var.cluster_identifier
  engine             = "aurora-postgresql"
  engine_mode        = "provisioned"
  engine_version     = var.engine_version
  database_name      = var.database_name
  master_username    = var.master_username
  master_password    = var.master_password
  port               = var.port
  storage_encrypted  = var.storage_encrypted
  kms_key_id         = var.kms_key_id

  db_subnet_group_name   = aws_db_subnet_group.this.name
  vpc_security_group_ids = local.security_group_ids

  backup_retention_period         = var.backup_retention_period
  preferred_backup_window         = var.preferred_backup_window
  preferred_maintenance_window    = var.preferred_maintenance_window
  copy_tags_to_snapshot           = var.copy_tags_to_snapshot
  deletion_protection             = var.deletion_protection
  skip_final_snapshot             = var.skip_final_snapshot
  final_snapshot_identifier       = var.skip_final_snapshot ? null : coalesce(var.final_snapshot_identifier, "${var.cluster_identifier}-final")
  apply_immediately               = var.apply_immediately
  enabled_cloudwatch_logs_exports = var.enabled_cloudwatch_logs_exports
  db_cluster_parameter_group_name = var.db_cluster_parameter_group_name
  iam_database_authentication_enabled = var.iam_database_authentication_enabled
  tags                                 = local.tags

  serverlessv2_scaling_configuration {
    max_capacity             = var.serverlessv2_max_capacity
    min_capacity             = var.serverlessv2_min_capacity
    seconds_until_auto_pause = var.seconds_until_auto_pause
  }

  depends_on = [aws_cloudwatch_log_group.postgresql]
}

resource "aws_rds_cluster_instance" "this" {
  count = var.instance_count

  cluster_identifier           = aws_rds_cluster.this.id
  identifier                   = "${var.cluster_identifier}-instance-${count.index + 1}"
  instance_class               = var.instance_class
  engine                       = aws_rds_cluster.this.engine
  engine_version               = aws_rds_cluster.this.engine_version
  apply_immediately            = var.apply_immediately
  auto_minor_version_upgrade   = var.auto_minor_version_upgrade
  db_parameter_group_name      = var.db_parameter_group_name
  monitoring_interval          = var.monitoring_interval
  monitoring_role_arn          = var.monitoring_role_arn
  performance_insights_enabled = var.performance_insights_enabled
  performance_insights_kms_key_id = var.performance_insights_kms_key_id
  publicly_accessible             = var.publicly_accessible
  tags                            = local.tags
}

resource "aws_vpc_endpoint" "s3" {
  count = var.create_s3_vpc_endpoint ? 1 : 0

  vpc_id            = var.vpc_id
  service_name      = "com.amazonaws.${data.aws_region.current.name}.s3"
  vpc_endpoint_type = "Gateway"
  route_table_ids   = var.s3_route_table_ids
  tags              = merge(local.tags, { Name = "${var.cluster_identifier}-s3-endpoint" })
}
