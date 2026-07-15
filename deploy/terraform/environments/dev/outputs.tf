output "alb_dns_name" {
  description = "Public DNS name for the development ALB."
  value       = module.alb.dns_name
}

output "ecs_cluster_name" {
  description = "ECS cluster name."
  value       = module.ecs_cluster.name
}

output "ecs_service_name" {
  description = "ECS service name."
  value       = module.ecs_service.service_name
}

output "task_definition_arn" {
  description = "Current task definition ARN."
  value       = module.ecs_task.task_definition_arn
}

output "attachments_bucket_name" {
  description = "S3 bucket name used for attachments."
  value       = module.attachments_bucket.bucket_name
}

output "aurora_endpoint" {
  description = "Aurora writer endpoint."
  value       = module.aurora.cluster_endpoint
}

output "redis_primary_endpoint" {
  description = "Redis primary endpoint."
  value       = module.redis.primary_endpoint_address
}

output "vpc_id" {
  description = "VPC ID for the dev environment."
  value       = module.network.vpc_id
}
