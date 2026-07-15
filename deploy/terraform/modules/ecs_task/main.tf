locals {
  tags = merge(
    {
      Name = var.family
    },
    var.tags
  )

  environment = [
    for key, value in var.environment :
    {
      name  = key
      value = value
    }
  ]

  secrets = [
    for key, value in var.secrets :
    {
      name      = key
      valueFrom = value
    }
  ]

  container_definition = merge(
    {
      name      = var.container_name
      image     = var.image
      essential = true
      portMappings = [
        {
          containerPort = var.container_port
          hostPort      = var.container_port
          protocol      = "tcp"
        }
      ]
      environment = local.environment
      secrets     = local.secrets
      logConfiguration = {
        logDriver = "awslogs"
        options = {
          awslogs-group         = var.create_log_group ? aws_cloudwatch_log_group.this[0].name : var.log_group_name
          awslogs-region        = var.aws_region
          awslogs-stream-prefix = var.log_stream_prefix
        }
      }
      readonlyRootFilesystem = var.readonly_root_filesystem
    },
    var.init_process_enabled ? {
      linuxParameters = {
        initProcessEnabled = true
      }
    } : {}
  )
}

resource "aws_cloudwatch_log_group" "this" {
  count = var.create_log_group ? 1 : 0

  name              = var.log_group_name != null ? var.log_group_name : "/ecs/${var.family}"
  retention_in_days = var.log_retention_in_days
  kms_key_id        = var.log_group_kms_key_id
  tags              = local.tags
}

resource "aws_ecs_task_definition" "this" {
  family                   = var.family
  cpu                      = var.cpu
  memory                   = var.memory
  network_mode             = "awsvpc"
  requires_compatibilities = var.requires_compatibilities
  execution_role_arn       = var.execution_role_arn
  task_role_arn            = var.task_role_arn

  runtime_platform {
    operating_system_family = var.operating_system_family
    cpu_architecture        = var.cpu_architecture
  }

  container_definitions = jsonencode([local.container_definition])

  tags = local.tags
}
