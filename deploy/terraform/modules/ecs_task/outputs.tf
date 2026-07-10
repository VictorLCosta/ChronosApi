output "task_definition_arn" {
  description = "Task definition ARN."
  value       = aws_ecs_task_definition.this.arn
}

output "task_definition_family" {
  description = "Task definition family."
  value       = aws_ecs_task_definition.this.family
}

output "container_name" {
  description = "Primary container name."
  value       = var.container_name
}

output "container_port" {
  description = "Primary container port."
  value       = var.container_port
}

output "log_group_name" {
  description = "CloudWatch log group name used by the task."
  value       = var.create_log_group ? aws_cloudwatch_log_group.this[0].name : var.log_group_name
}
