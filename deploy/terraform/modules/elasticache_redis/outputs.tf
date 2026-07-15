output "replication_group_id" {
  description = "ElastiCache replication group ID."
  value       = aws_elasticache_replication_group.this.replication_group_id
}

output "replication_group_arn" {
  description = "ElastiCache replication group ARN."
  value       = aws_elasticache_replication_group.this.arn
}

output "primary_endpoint_address" {
  description = "Primary endpoint address for Redis writes."
  value       = aws_elasticache_replication_group.this.primary_endpoint_address
}

output "reader_endpoint_address" {
  description = "Reader endpoint address for Redis reads."
  value       = aws_elasticache_replication_group.this.reader_endpoint_address
}

output "port" {
  description = "Port exposed by the Redis replication group."
  value       = aws_elasticache_replication_group.this.port
}

output "subnet_group_name" {
  description = "ElastiCache subnet group name."
  value       = aws_elasticache_subnet_group.this.name
}

output "security_group_id" {
  description = "Created security group ID, when enabled."
  value       = var.create_security_group ? aws_security_group.this[0].id : null
}

output "security_group_ids" {
  description = "All security groups attached to the replication group."
  value       = local.security_group_ids
}

output "parameter_group_name" {
  description = "Parameter group attached to the replication group."
  value       = var.create_parameter_group ? aws_elasticache_parameter_group.this[0].name : var.parameter_group_name
}

output "engine_version_actual" {
  description = "Actual engine version running on Redis."
  value       = aws_elasticache_replication_group.this.engine_version_actual
}

output "configuration_endpoint_address" {
  description = "Configuration endpoint for cluster mode, when available."
  value       = try(aws_elasticache_replication_group.this.configuration_endpoint_address, null)
}
