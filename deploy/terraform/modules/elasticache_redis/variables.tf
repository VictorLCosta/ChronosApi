variable "replication_group_id" {
  type        = string
  description = "Unique identifier for the ElastiCache replication group."

  validation {
    condition     = can(regex("^[a-z0-9-]+$", var.replication_group_id))
    error_message = "The replication group ID must contain only lowercase letters, numbers, and hyphens."
  }
}

variable "description" {
  type        = string
  description = "Description for the ElastiCache replication group."
}

variable "vpc_id" {
  type        = string
  description = "VPC ID where Redis will be deployed."
}

variable "subnet_ids" {
  type        = list(string)
  description = "Private subnet IDs used by the ElastiCache subnet group."

  validation {
    condition     = length(var.subnet_ids) >= 2
    error_message = "Provide at least two subnets in different AZs for ElastiCache."
  }
}

variable "subnet_group_name" {
  type        = string
  description = "Optional subnet group name. If null, one is generated."
  default     = null
}

variable "create_security_group" {
  type        = bool
  description = "Whether the module should create a dedicated security group."
  default     = true
}

variable "additional_security_group_ids" {
  type        = list(string)
  description = "Additional security groups attached to the replication group."
  default     = []
}

variable "allowed_security_groups" {
  type = list(object({
    security_group_id = string
    description       = optional(string, "Allow Redis access from a trusted security group.")
  }))
  description = "Security groups allowed to connect to Redis on the cache port."
  default     = []
}

variable "allowed_cidr_blocks" {
  type = list(object({
    cidr_block  = string
    description = optional(string, "Allow Redis access from a trusted CIDR block.")
  }))
  description = "CIDR blocks allowed to connect to Redis on the cache port."
  default     = []
}

variable "engine_version" {
  type        = string
  description = "Redis engine version."
  default     = "7.1"
}

variable "port" {
  type        = number
  description = "Port used by Redis."
  default     = 6379
}

variable "node_type" {
  type        = string
  description = "Instance class for cache nodes."
  default     = "cache.t4g.small"
}

variable "automatic_failover_enabled" {
  type        = bool
  description = "Whether automatic failover is enabled."
  default     = true
}

variable "multi_az_enabled" {
  type        = bool
  description = "Whether Multi-AZ is enabled."
  default     = true
}

variable "auto_minor_version_upgrade" {
  type        = bool
  description = "Whether to enable automatic minor version upgrades."
  default     = true
}

variable "apply_immediately" {
  type        = bool
  description = "Whether modifications are applied immediately."
  default     = false
}

variable "at_rest_encryption_enabled" {
  type        = bool
  description = "Whether to enable encryption at rest."
  default     = true
}

variable "transit_encryption_enabled" {
  type        = bool
  description = "Whether to enable encryption in transit."
  default     = true
}

variable "auth_token" {
  type        = string
  description = "Redis AUTH token used when transit encryption is enabled."
  sensitive   = true
  default     = null

  validation {
    condition     = var.auth_token == null || var.transit_encryption_enabled
    error_message = "auth_token requires transit_encryption_enabled = true."
  }
}

variable "kms_key_id" {
  type        = string
  description = "KMS key ARN or ID used to encrypt Redis at rest."
  default     = null
}

variable "maintenance_window" {
  type        = string
  description = "Weekly time range in UTC during which maintenance can occur."
  default     = "sun:05:00-sun:06:00"
}

variable "snapshot_retention_limit" {
  type        = number
  description = "Number of days to retain automatic snapshots."
  default     = 1
}

variable "snapshot_window" {
  type        = string
  description = "Daily time range in UTC during which snapshots are created."
  default     = "03:00-04:00"
}

variable "notification_topic_arn" {
  type        = string
  description = "SNS topic ARN used for cache notifications."
  default     = null
}

variable "cluster_mode_enabled" {
  type        = bool
  description = "Whether to enable cluster mode."
  default     = false
}

variable "num_cache_clusters" {
  type        = number
  description = "Number of cache clusters when cluster mode is disabled."
  default     = 2

  validation {
    condition     = var.num_cache_clusters >= 1
    error_message = "num_cache_clusters must be at least 1."
  }
}

variable "num_node_groups" {
  type        = number
  description = "Number of node groups when cluster mode is enabled."
  default     = 1

  validation {
    condition     = var.num_node_groups >= 1
    error_message = "num_node_groups must be at least 1."
  }
}

variable "replicas_per_node_group" {
  type        = number
  description = "Number of replicas per node group when cluster mode is enabled."
  default     = 1

  validation {
    condition     = var.replicas_per_node_group >= 0
    error_message = "replicas_per_node_group cannot be negative."
  }
}

variable "preferred_cache_cluster_azs" {
  type        = list(string)
  description = "Preferred AZs for cache clusters when cluster mode is disabled."
  default     = []
}

variable "final_snapshot_identifier" {
  type        = string
  description = "Optional final snapshot identifier used when replacing or deleting the replication group."
  default     = null
}

variable "data_tiering_enabled" {
  type        = bool
  description = "Whether to enable data tiering for supported node types."
  default     = false
}

variable "network_type" {
  type        = string
  description = "Network type for the replication group."
  default     = "ipv4"
}

variable "ip_discovery" {
  type        = string
  description = "IP discovery mode for the replication group."
  default     = "ipv4"
}

variable "create_parameter_group" {
  type        = bool
  description = "Whether the module should create a custom parameter group."
  default     = false
}

variable "parameter_group_name" {
  type        = string
  description = "Existing or generated parameter group name."
  default     = null
}

variable "parameter_group_family" {
  type        = string
  description = "Parameter group family used when creating a custom parameter group."
  default     = "redis7"
}

variable "parameters" {
  type = list(object({
    name  = string
    value = string
  }))
  description = "Custom parameter overrides for Redis."
  default     = []
}

variable "create_cloudwatch_log_group" {
  type        = bool
  description = "Whether the module should create CloudWatch log groups for Redis log delivery."
  default     = true
}

variable "cloudwatch_log_group_retention_in_days" {
  type        = number
  description = "Retention period for Redis CloudWatch log groups."
  default     = 30
}

variable "cloudwatch_log_group_kms_key_id" {
  type        = string
  description = "Optional KMS key for Redis CloudWatch log groups."
  default     = null
}

variable "log_delivery_configuration" {
  type = object({
    engine = object({
      enabled          = bool
      destination_type = string
      destination      = optional(string)
      log_format       = string
    })
    slow = object({
      enabled          = bool
      destination_type = string
      destination      = optional(string)
      log_format       = string
    })
  })
  description = "Log delivery settings for engine and slow logs."
  default = {
    engine = {
      enabled          = true
      destination_type = "cloudwatch-logs"
      destination      = null
      log_format       = "text"
    }
    slow = {
      enabled          = true
      destination_type = "cloudwatch-logs"
      destination      = null
      log_format       = "json"
    }
  }
}

variable "tags" {
  type        = map(string)
  description = "Tags applied to all Redis resources."
  default     = {}
}
