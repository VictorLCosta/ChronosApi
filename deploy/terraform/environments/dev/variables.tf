variable "aws_region" {
  type        = string
  description = "AWS region for the dev environment."
}

variable "project_name" {
  type        = string
  description = "Project name used in resource naming."
  default     = "chronos-api"
}

variable "environment" {
  type        = string
  description = "Environment name."
  default     = "dev"
}

variable "vpc_cidr" {
  type        = string
  description = "CIDR block for the VPC."
}

variable "public_subnets" {
  type = map(object({
    cidr_block = string
    az         = string
  }))
  description = "Public subnet definitions."
}

variable "private_subnets" {
  type = map(object({
    cidr_block = string
    az         = string
  }))
  description = "Private subnet definitions."
}

variable "attachments_bucket_name" {
  type        = string
  description = "Globally unique bucket name for user attachments."
}

variable "attachments_lifecycle_rules" {
  type = list(object({
    id                                 = string
    enabled                            = bool
    prefix                             = optional(string, null)
    expiration_days                    = optional(number, null)
    noncurrent_version_expiration_days = optional(number, null)
  }))
  description = "Lifecycle rules for the attachments bucket."
  default     = []
}

variable "attachments_cors_rules" {
  type = list(object({
    allowed_headers = list(string)
    allowed_methods = list(string)
    allowed_origins = list(string)
    expose_headers  = optional(list(string), [])
    max_age_seconds = optional(number, 3000)
  }))
  description = "Optional CORS rules for direct attachment uploads."
  default     = []
}

variable "db_name" {
  type        = string
  description = "Aurora database name."
  default     = "chronos"
}

variable "db_username" {
  type        = string
  description = "Aurora master username."
  default     = "chronos"
}

variable "db_password" {
  type        = string
  description = "Aurora master password."
  sensitive   = true
}

variable "aurora_engine_version" {
  type        = string
  description = "Aurora PostgreSQL engine version."
  default     = "13.12"
}

variable "aurora_instance_count" {
  type        = number
  description = "Number of Aurora instances."
  default     = 1
}

variable "aurora_serverlessv2_min_capacity" {
  type        = number
  description = "Minimum ACUs for Aurora Serverless v2."
  default     = 0.5
}

variable "aurora_serverlessv2_max_capacity" {
  type        = number
  description = "Maximum ACUs for Aurora Serverless v2."
  default     = 1
}

variable "aurora_backup_retention_period" {
  type        = number
  description = "Aurora backup retention in days."
  default     = 1
}

variable "redis_engine_version" {
  type        = string
  description = "Redis engine version."
  default     = "7.1"
}

variable "redis_node_type" {
  type        = string
  description = "Redis node type."
  default     = "cache.t4g.small"
}

variable "redis_num_cache_clusters" {
  type        = number
  description = "Number of Redis cache nodes when cluster mode is disabled."
  default     = 2
}

variable "redis_automatic_failover_enabled" {
  type        = bool
  description = "Whether Redis automatic failover is enabled."
  default     = true
}

variable "redis_multi_az_enabled" {
  type        = bool
  description = "Whether Redis Multi-AZ is enabled."
  default     = true
}

variable "redis_snapshot_retention_limit" {
  type        = number
  description = "Redis snapshot retention in days."
  default     = 1
}

variable "redis_auth_token" {
  type        = string
  description = "Redis AUTH token used with transit encryption."
  sensitive   = true
}

variable "container_image" {
  type        = string
  description = "Container image URI for the API."
}

variable "container_name" {
  type        = string
  description = "Container name in the task definition."
  default     = "chronos-api"
}

variable "container_port" {
  type        = number
  description = "Application container port."
  default     = 8080
}

variable "task_cpu" {
  type        = string
  description = "Fargate task CPU."
  default     = "256"
}

variable "task_memory" {
  type        = string
  description = "Fargate task memory."
  default     = "512"
}

variable "service_desired_count" {
  type        = number
  description = "Desired number of ECS tasks."
  default     = 1
}

variable "autoscaling_min_capacity" {
  type        = number
  description = "Minimum number of ECS tasks."
  default     = 1
}

variable "autoscaling_max_capacity" {
  type        = number
  description = "Maximum number of ECS tasks."
  default     = 2
}

variable "memory_target_value" {
  type        = number
  description = "Target average memory utilization percentage."
  default     = 70
}

variable "scale_in_cooldown" {
  type        = number
  description = "Scale in cooldown in seconds."
  default     = 120
}

variable "scale_out_cooldown" {
  type        = number
  description = "Scale out cooldown in seconds."
  default     = 60
}

variable "listener_rule_priority" {
  type        = number
  description = "Priority for the ALB listener rule."
  default     = 100
}

variable "jwt_key" {
  type        = string
  description = "JWT signing key for the API."
  sensitive   = true
}

variable "jwt_issuer" {
  type        = string
  description = "JWT issuer."
  default     = "ChronosApi"
}

variable "jwt_audience" {
  type        = string
  description = "JWT audience."
  default     = "ChronosApi.Client"
}

variable "jwt_duration_minutes" {
  type        = number
  description = "JWT access token duration in minutes."
  default     = 60
}

variable "jwt_refresh_token_duration_days" {
  type        = number
  description = "JWT refresh token duration in days."
  default     = 90
}
