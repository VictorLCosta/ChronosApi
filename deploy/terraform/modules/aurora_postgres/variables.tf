variable "cluster_identifier" {
  type        = string
  description = "Unique identifier for the Aurora cluster."

  validation {
    condition     = can(regex("^[a-z0-9-]+$", var.cluster_identifier))
    error_message = "The cluster identifier must contain only lowercase letters, numbers, and hyphens."
  }
}

variable "vpc_id" {
  type        = string
  description = "VPC ID where Aurora will be deployed."
}

variable "subnet_ids" {
  type        = list(string)
  description = "Private subnet IDs used by the DB subnet group."

  validation {
    condition     = length(var.subnet_ids) >= 2
    error_message = "Provide at least two subnets in different AZs for Aurora."
  }
}

variable "db_subnet_group_name" {
  type        = string
  description = "Optional DB subnet group name. If null, one is generated."
  default     = null
}

variable "create_security_group" {
  type        = bool
  description = "Whether the module should create a dedicated security group."
  default     = true
}

variable "additional_security_group_ids" {
  type        = list(string)
  description = "Additional security groups attached to the cluster."
  default     = []
}

variable "allowed_security_groups" {
  type = list(object({
    security_group_id = string
    description       = optional(string, "Allow PostgreSQL access from a trusted security group.")
  }))
  description = "Security groups allowed to connect to Aurora on the database port."
  default     = []
}

variable "allowed_cidr_blocks" {
  type = list(object({
    cidr_block  = string
    description = optional(string, "Allow PostgreSQL access from a trusted CIDR block.")
  }))
  description = "CIDR blocks allowed to connect to Aurora on the database port."
  default     = []
}

variable "engine_version" {
  type        = string
  description = "Aurora PostgreSQL engine version."
  default     = "13.6"
}

variable "database_name" {
  type        = string
  description = "Initial database name to create."

  validation {
    condition     = can(regex("^[A-Za-z][A-Za-z0-9_]*$", var.database_name))
    error_message = "The database name must start with a letter and contain only letters, numbers, and underscores."
  }
}

variable "master_username" {
  type        = string
  description = "Master username for the Aurora cluster."

  validation {
    condition     = can(regex("^[A-Za-z][A-Za-z0-9_]*$", var.master_username))
    error_message = "The master username must start with a letter and contain only letters, numbers, and underscores."
  }
}

variable "master_password" {
  type        = string
  description = "Master password for the Aurora cluster."
  sensitive   = true

  validation {
    condition     = length(var.master_password) >= 8
    error_message = "The master password must have at least 8 characters."
  }
}

variable "port" {
  type        = number
  description = "Port used by Aurora PostgreSQL."
  default     = 5432
}

variable "storage_encrypted" {
  type        = bool
  description = "Whether to enable storage encryption."
  default     = true
}

variable "kms_key_id" {
  type        = string
  description = "KMS key ARN or ID used to encrypt the cluster. Leave null to use the AWS managed key."
  default     = null
}

variable "backup_retention_period" {
  type        = number
  description = "Number of days to retain backups."
  default     = 7
}

variable "preferred_backup_window" {
  type        = string
  description = "Daily time range in UTC during which automated backups are created."
  default     = "03:00-04:00"
}

variable "preferred_maintenance_window" {
  type        = string
  description = "Weekly time range in UTC during which maintenance can occur."
  default     = "sun:04:00-sun:05:00"
}

variable "copy_tags_to_snapshot" {
  type        = bool
  description = "Whether to copy tags from the cluster to snapshots."
  default     = true
}

variable "deletion_protection" {
  type        = bool
  description = "Whether to enable deletion protection."
  default     = true
}

variable "skip_final_snapshot" {
  type        = bool
  description = "Whether to skip the final snapshot when deleting the cluster."
  default     = false
}

variable "final_snapshot_identifier" {
  type        = string
  description = "Identifier for the final snapshot created when the cluster is deleted."
  default     = null
}

variable "apply_immediately" {
  type        = bool
  description = "Whether to apply changes immediately."
  default     = false
}

variable "serverlessv2_min_capacity" {
  type        = number
  description = "Minimum Aurora Serverless v2 capacity units."
  default     = 0.0
}

variable "serverlessv2_max_capacity" {
  type        = number
  description = "Maximum Aurora Serverless v2 capacity units."
  default     = 1.0
}

variable "seconds_until_auto_pause" {
  type        = number
  description = "How long Aurora Serverless v2 waits before auto-pausing."
  default     = 3600
}

variable "instance_count" {
  type        = number
  description = "Number of Aurora cluster instances to create."
  default     = 1

  validation {
    condition     = var.instance_count >= 1
    error_message = "You must create at least one cluster instance."
  }
}

variable "instance_class" {
  type        = string
  description = "Instance class for cluster instances."
  default     = "db.serverless"
}

variable "publicly_accessible" {
  type        = bool
  description = "Whether cluster instances should receive public accessibility."
  default     = false
}

variable "auto_minor_version_upgrade" {
  type        = bool
  description = "Whether to enable automatic minor version upgrades for instances."
  default     = true
}

variable "monitoring_interval" {
  type        = number
  description = "Enhanced monitoring interval in seconds. Set to 0 to disable."
  default     = 0
}

variable "monitoring_role_arn" {
  type        = string
  description = "IAM role ARN used for RDS enhanced monitoring."
  default     = null
}

variable "performance_insights_enabled" {
  type        = bool
  description = "Whether to enable Performance Insights."
  default     = false
}

variable "performance_insights_kms_key_id" {
  type        = string
  description = "KMS key for Performance Insights encryption."
  default     = null
}

variable "db_parameter_group_name" {
  type        = string
  description = "Optional DB parameter group name for Aurora instances."
  default     = null
}

variable "db_cluster_parameter_group_name" {
  type        = string
  description = "Optional DB cluster parameter group name."
  default     = null
}

variable "iam_database_authentication_enabled" {
  type        = bool
  description = "Whether to enable IAM database authentication."
  default     = false
}

variable "enabled_cloudwatch_logs_exports" {
  type        = list(string)
  description = "List of log types to export to CloudWatch Logs."
  default     = ["postgresql"]
}

variable "create_cloudwatch_log_group" {
  type        = bool
  description = "Whether the module should create the CloudWatch log group used by exported database logs."
  default     = true
}

variable "cloudwatch_log_group_retention_in_days" {
  type        = number
  description = "Retention period for the CloudWatch log group."
  default     = 30
}

variable "cloudwatch_log_group_kms_key_id" {
  type        = string
  description = "Optional KMS key for the CloudWatch log group."
  default     = null
}

variable "create_s3_vpc_endpoint" {
  type        = bool
  description = "Whether to create a private VPC endpoint for S3."
  default     = false
}

variable "s3_route_table_ids" {
  type        = list(string)
  description = "Route table IDs associated with the S3 gateway endpoint."
  default     = []
}

variable "tags" {
  type        = map(string)
  description = "Tags applied to all Aurora resources."
  default     = {}
}
