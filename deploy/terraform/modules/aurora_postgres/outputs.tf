output "cluster_id" {
  description = "Aurora cluster ID."
  value       = aws_rds_cluster.this.id
}

output "cluster_arn" {
  description = "Aurora cluster ARN."
  value       = aws_rds_cluster.this.arn
}

output "cluster_identifier" {
  description = "Aurora cluster identifier."
  value       = aws_rds_cluster.this.cluster_identifier
}

output "cluster_endpoint" {
  description = "Writer endpoint for the Aurora cluster."
  value       = aws_rds_cluster.this.endpoint
}

output "cluster_reader_endpoint" {
  description = "Reader endpoint for the Aurora cluster."
  value       = aws_rds_cluster.this.reader_endpoint
}

output "cluster_port" {
  description = "Port exposed by the Aurora cluster."
  value       = aws_rds_cluster.this.port
}

output "database_name" {
  description = "Initial database name."
  value       = aws_rds_cluster.this.database_name
}

output "db_subnet_group_name" {
  description = "Database subnet group name."
  value       = aws_db_subnet_group.this.name
}

output "security_group_id" {
  description = "Created security group ID, when enabled."
  value       = var.create_security_group ? aws_security_group.this[0].id : null
}

output "security_group_ids" {
  description = "All security groups attached to the cluster."
  value       = local.security_group_ids
}

output "instance_ids" {
  description = "Aurora cluster instance IDs."
  value       = aws_rds_cluster_instance.this[*].id
}

output "instance_arns" {
  description = "Aurora cluster instance ARNs."
  value       = aws_rds_cluster_instance.this[*].arn
}

output "s3_vpc_endpoint_id" {
  description = "Gateway VPC endpoint ID for S3, when enabled."
  value       = var.create_s3_vpc_endpoint ? aws_vpc_endpoint.s3[0].id : null
}

output "s3_vpc_endpoint_prefix_list_id" {
  description = "Prefix list ID for the S3 VPC endpoint, when enabled."
  value       = var.create_s3_vpc_endpoint ? aws_vpc_endpoint.s3[0].prefix_list_id : null
}

output "s3_service_name" {
  description = "AWS service name used by the S3 VPC endpoint."
  value       = "com.amazonaws.${data.aws_region.current.name}.s3"
}

output "s3_regional_endpoint_url" {
  description = "Regional S3 endpoint URL used by workloads inside the VPC."
  value       = "https://s3.${data.aws_region.current.name}.amazonaws.com"
}
