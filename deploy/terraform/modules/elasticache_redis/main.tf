locals {
  tags = merge(
    {
      Name = var.replication_group_id
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

resource "aws_elasticache_subnet_group" "this" {
  name       = var.subnet_group_name != null ? var.subnet_group_name : "${var.replication_group_id}-subnet-group"
  subnet_ids = var.subnet_ids
  tags       = local.tags
}

resource "aws_security_group" "this" {
  count       = var.create_security_group ? 1 : 0
  name        = "${var.replication_group_id}-sg"
  description = "Security group for ${var.replication_group_id} ElastiCache Redis replication group."
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

resource "aws_cloudwatch_log_group" "engine" {
  count = var.create_cloudwatch_log_group && var.log_delivery_configuration.engine.enabled ? 1 : 0

  name              = "/aws/elasticache/${var.replication_group_id}/engine-log"
  retention_in_days = var.cloudwatch_log_group_retention_in_days
  kms_key_id        = var.cloudwatch_log_group_kms_key_id
  tags              = local.tags
}

resource "aws_cloudwatch_log_group" "slow" {
  count = var.create_cloudwatch_log_group && var.log_delivery_configuration.slow.enabled ? 1 : 0

  name              = "/aws/elasticache/${var.replication_group_id}/slow-log"
  retention_in_days = var.cloudwatch_log_group_retention_in_days
  kms_key_id        = var.cloudwatch_log_group_kms_key_id
  tags              = local.tags
}

resource "aws_elasticache_parameter_group" "this" {
  count = var.create_parameter_group ? 1 : 0

  name   = var.parameter_group_name != null ? var.parameter_group_name : "${var.replication_group_id}-params"
  family = var.parameter_group_family

  dynamic "parameter" {
    for_each = var.parameters
    content {
      name  = parameter.value.name
      value = parameter.value.value
    }
  }
}

resource "aws_elasticache_replication_group" "this" {
  replication_group_id          = var.replication_group_id
  description                   = var.description
  engine                        = "redis"
  engine_version                = var.engine_version
  port                          = var.port
  node_type                     = var.node_type
  parameter_group_name          = var.create_parameter_group ? aws_elasticache_parameter_group.this[0].name : var.parameter_group_name
  subnet_group_name             = aws_elasticache_subnet_group.this.name
  security_group_ids            = local.security_group_ids
  automatic_failover_enabled    = var.automatic_failover_enabled
  multi_az_enabled              = var.multi_az_enabled
  auto_minor_version_upgrade    = var.auto_minor_version_upgrade
  apply_immediately             = var.apply_immediately
  at_rest_encryption_enabled    = var.at_rest_encryption_enabled
  transit_encryption_enabled    = var.transit_encryption_enabled
  auth_token                    = var.auth_token
  kms_key_id                    = var.kms_key_id
  maintenance_window            = var.maintenance_window
  snapshot_retention_limit      = var.snapshot_retention_limit
  snapshot_window               = var.snapshot_window
  notification_topic_arn        = var.notification_topic_arn
  preferred_cache_cluster_azs   = var.preferred_cache_cluster_azs
  num_cache_clusters            = var.cluster_mode_enabled ? null : var.num_cache_clusters
  replicas_per_node_group       = var.cluster_mode_enabled ? var.replicas_per_node_group : null
  num_node_groups               = var.cluster_mode_enabled ? var.num_node_groups : null
  final_snapshot_identifier     = var.final_snapshot_identifier
  data_tiering_enabled          = var.data_tiering_enabled
  network_type                  = var.network_type
  ip_discovery                  = var.ip_discovery
  tags                          = local.tags

  dynamic "log_delivery_configuration" {
    for_each = {
      for log_type, config in var.log_delivery_configuration :
      log_type => config if config.enabled
    }

    content {
      destination      = log_delivery_configuration.value.destination_type == "cloudwatch-logs" ? (
        log_delivery_configuration.key == "engine"
        ? aws_cloudwatch_log_group.engine[0].name
        : aws_cloudwatch_log_group.slow[0].name
      ) : log_delivery_configuration.value.destination
      destination_type = log_delivery_configuration.value.destination_type
      log_format       = log_delivery_configuration.value.log_format
      log_type         = log_delivery_configuration.key
    }
  }

  depends_on = [
    aws_cloudwatch_log_group.engine,
    aws_cloudwatch_log_group.slow
  ]
}
