output "service_id" {
  description = "ECS service ID."
  value       = aws_ecs_service.this.id
}

output "service_name" {
  description = "ECS service name."
  value       = aws_ecs_service.this.name
}

output "service_arn" {
  description = "ECS service ARN."
  value       = aws_ecs_service.this.arn
}

output "target_group_arn" {
  description = "Target group ARN attached to the service."
  value       = local.target_group_arn
}

output "target_group_name" {
  description = "Created target group name, when enabled."
  value       = var.create_target_group ? aws_lb_target_group.this[0].name : null
}

output "autoscaling_target_resource_id" {
  description = "Application Auto Scaling resource ID for the ECS service."
  value       = var.enable_autoscaling ? aws_appautoscaling_target.this[0].resource_id : null
}

output "memory_target_tracking_policy_arn" {
  description = "ARN of the memory target tracking autoscaling policy."
  value       = var.enable_autoscaling ? aws_appautoscaling_policy.memory_target_tracking[0].arn : null
}
