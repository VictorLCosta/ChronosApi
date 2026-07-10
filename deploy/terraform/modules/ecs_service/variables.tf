variable "name" {
  type        = string
  description = "ECS service name."
}

variable "cluster_arn" {
  type        = string
  description = "ARN of the ECS cluster."
}

variable "cluster_name" {
  type        = string
  description = "Name of the ECS cluster."
}

variable "task_definition_arn" {
  type        = string
  description = "ARN of the task definition used by the service."
}

variable "desired_count" {
  type        = number
  description = "Desired number of tasks."
  default     = 1
}

variable "launch_type" {
  type        = string
  description = "Launch type used when capacity providers are not configured."
  default     = "FARGATE"
}

variable "platform_version" {
  type        = string
  description = "Fargate platform version."
  default     = "LATEST"
}

variable "capacity_provider_strategy" {
  type = list(object({
    capacity_provider = string
    weight            = optional(number, 1)
    base              = optional(number, 0)
  }))
  description = "Optional capacity provider strategy."
  default     = null
}

variable "subnet_ids" {
  type        = list(string)
  description = "Subnets used by the ECS service network configuration."
}

variable "security_group_ids" {
  type        = list(string)
  description = "Security groups attached to the ECS service."
  default     = []
}

variable "assign_public_ip" {
  type        = bool
  description = "Whether tasks should receive public IPs."
  default     = false
}

variable "enable_ecs_managed_tags" {
  type        = bool
  description = "Whether to enable ECS managed tags."
  default     = true
}

variable "propagate_tags" {
  type        = string
  description = "Whether to propagate tags from SERVICE or TASK_DEFINITION."
  default     = "SERVICE"
}

variable "enable_execute_command" {
  type        = bool
  description = "Whether ECS Exec is enabled."
  default     = false
}

variable "force_new_deployment" {
  type        = bool
  description = "Whether to force a new deployment on each apply."
  default     = false
}

variable "wait_for_steady_state" {
  type        = bool
  description = "Whether Terraform should wait for the service to become stable."
  default     = true
}

variable "deployment_minimum_healthy_percent" {
  type        = number
  description = "Lower limit on the number of running tasks during deployment."
  default     = 100
}

variable "deployment_maximum_percent" {
  type        = number
  description = "Upper limit on the number of running tasks during deployment."
  default     = 200
}

variable "deployment_circuit_breaker_enabled" {
  type        = bool
  description = "Whether to enable the ECS deployment circuit breaker."
  default     = true
}

variable "deployment_circuit_breaker_rollback" {
  type        = bool
  description = "Whether failed deployments should roll back automatically."
  default     = true
}

variable "attach_to_load_balancer" {
  type        = bool
  description = "Whether to attach the service to a target group."
  default     = true
}

variable "container_name" {
  type        = string
  description = "Container name exposed by the task definition."
}

variable "container_port" {
  type        = number
  description = "Container port exposed by the task definition."
}

variable "create_target_group" {
  type        = bool
  description = "Whether the module should create the target group."
  default     = true
}

variable "target_group_arn" {
  type        = string
  description = "Existing target group ARN to use when create_target_group is false."
  default     = null
}

variable "vpc_id" {
  type        = string
  description = "VPC ID used when creating a target group."
  default     = null
}

variable "target_group_protocol" {
  type        = string
  description = "Protocol used by the target group."
  default     = "HTTP"
}

variable "deregistration_delay" {
  type        = number
  description = "Deregistration delay in seconds for the target group."
  default     = 30
}

variable "health_check_path" {
  type        = string
  description = "Health check path used by the target group."
  default     = "/health/ready"
}

variable "health_check_protocol" {
  type        = string
  description = "Health check protocol used by the target group."
  default     = "HTTP"
}

variable "health_check_matcher" {
  type        = string
  description = "Matcher for successful health check responses."
  default     = "200-399"
}

variable "health_check_interval" {
  type        = number
  description = "Health check interval in seconds."
  default     = 30
}

variable "health_check_timeout" {
  type        = number
  description = "Health check timeout in seconds."
  default     = 5
}

variable "health_check_healthy_threshold" {
  type        = number
  description = "Number of successful checks required for healthy status."
  default     = 2
}

variable "health_check_unhealthy_threshold" {
  type        = number
  description = "Number of failed checks required for unhealthy status."
  default     = 3
}

variable "health_check_grace_period_seconds" {
  type        = number
  description = "Grace period before LB health checks affect the service."
  default     = 60
}

variable "create_listener_rule" {
  type        = bool
  description = "Whether to create a listener rule for the service target group."
  default     = false
}

variable "listener_arn" {
  type        = string
  description = "Listener ARN used when creating a listener rule."
  default     = null
}

variable "listener_rule_priority" {
  type        = number
  description = "Priority of the listener rule."
  default     = null
}

variable "listener_rule_path_patterns" {
  type        = list(string)
  description = "Path patterns matched by the listener rule."
  default     = ["/*"]
}

variable "listener_rule_host_headers" {
  type        = list(string)
  description = "Optional host headers matched by the listener rule."
  default     = []
}

variable "enable_autoscaling" {
  type        = bool
  description = "Whether to enable Application Auto Scaling for the ECS service."
  default     = true
}

variable "autoscaling_min_capacity" {
  type        = number
  description = "Minimum desired task count when autoscaling is enabled."
  default     = 1
}

variable "autoscaling_max_capacity" {
  type        = number
  description = "Maximum desired task count when autoscaling is enabled."
  default     = 4
}

variable "memory_target_value" {
  type        = number
  description = "Target average memory utilization percentage for target tracking."
  default     = 70
}

variable "scale_in_cooldown" {
  type        = number
  description = "Cooldown in seconds after scale in."
  default     = 120
}

variable "scale_out_cooldown" {
  type        = number
  description = "Cooldown in seconds after scale out."
  default     = 60
}

variable "tags" {
  type        = map(string)
  description = "Tags applied to ECS service resources."
  default     = {}
}
