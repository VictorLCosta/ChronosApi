locals {
  tags = merge(
    {
      Name = var.name
    },
    var.tags
  )

  target_group_arn = var.create_target_group ? aws_lb_target_group.this[0].arn : var.target_group_arn
}

resource "aws_lb_target_group" "this" {
  count = var.create_target_group ? 1 : 0

  name                 = substr("${var.name}-tg", 0, 32)
  port                 = var.container_port
  protocol             = var.target_group_protocol
  target_type          = "ip"
  vpc_id               = var.vpc_id
  deregistration_delay = var.deregistration_delay

  health_check {
    enabled             = true
    path                = var.health_check_path
    protocol            = var.health_check_protocol
    matcher             = var.health_check_matcher
    interval            = var.health_check_interval
    timeout             = var.health_check_timeout
    healthy_threshold   = var.health_check_healthy_threshold
    unhealthy_threshold = var.health_check_unhealthy_threshold
  }

  tags = local.tags
}

resource "aws_lb_listener_rule" "this" {
  count = var.create_listener_rule ? 1 : 0

  listener_arn = var.listener_arn
  priority     = var.listener_rule_priority

  action {
    type             = "forward"
    target_group_arn = local.target_group_arn
  }

  condition {
    path_pattern {
      values = var.listener_rule_path_patterns
    }
  }

  dynamic "condition" {
    for_each = length(var.listener_rule_host_headers) > 0 ? [1] : []
    content {
      host_header {
        values = var.listener_rule_host_headers
      }
    }
  }

  tags = local.tags
}

resource "aws_ecs_service" "this" {
  name                               = var.name
  cluster                            = var.cluster_arn
  task_definition                    = var.task_definition_arn
  desired_count                      = var.desired_count
  launch_type                        = var.capacity_provider_strategy == null ? var.launch_type : null
  platform_version                   = var.platform_version
  scheduling_strategy                = "REPLICA"
  enable_ecs_managed_tags            = var.enable_ecs_managed_tags
  health_check_grace_period_seconds  = var.attach_to_load_balancer ? var.health_check_grace_period_seconds : null
  wait_for_steady_state              = var.wait_for_steady_state
  deployment_minimum_healthy_percent = var.deployment_minimum_healthy_percent
  deployment_maximum_percent         = var.deployment_maximum_percent
  propagate_tags                     = var.propagate_tags
  enable_execute_command             = var.enable_execute_command
  force_new_deployment               = var.force_new_deployment

  dynamic "capacity_provider_strategy" {
    for_each = var.capacity_provider_strategy == null ? [] : var.capacity_provider_strategy
    content {
      capacity_provider = capacity_provider_strategy.value.capacity_provider
      weight            = capacity_provider_strategy.value.weight
      base              = capacity_provider_strategy.value.base
    }
  }

  network_configuration {
    assign_public_ip = var.assign_public_ip
    security_groups  = var.security_group_ids
    subnets          = var.subnet_ids
  }

  dynamic "load_balancer" {
    for_each = var.attach_to_load_balancer ? [1] : []
    content {
      container_name   = var.container_name
      container_port   = var.container_port
      target_group_arn = local.target_group_arn
    }
  }

  deployment_circuit_breaker {
    enable   = var.deployment_circuit_breaker_enabled
    rollback = var.deployment_circuit_breaker_rollback
  }

  tags = local.tags

  lifecycle {
    ignore_changes = [desired_count]
  }

  depends_on = [aws_lb_listener_rule.this]
}

resource "aws_appautoscaling_target" "this" {
  count = var.enable_autoscaling ? 1 : 0

  max_capacity       = var.autoscaling_max_capacity
  min_capacity       = var.autoscaling_min_capacity
  resource_id        = "service/${var.cluster_name}/${aws_ecs_service.this.name}"
  scalable_dimension = "ecs:service:DesiredCount"
  service_namespace  = "ecs"
}

resource "aws_appautoscaling_policy" "memory_target_tracking" {
  count = var.enable_autoscaling ? 1 : 0

  name               = "${var.name}-memory-target-tracking"
  policy_type        = "TargetTrackingScaling"
  resource_id        = aws_appautoscaling_target.this[0].resource_id
  scalable_dimension = aws_appautoscaling_target.this[0].scalable_dimension
  service_namespace  = aws_appautoscaling_target.this[0].service_namespace

  target_tracking_scaling_policy_configuration {
    target_value       = var.memory_target_value
    scale_in_cooldown  = var.scale_in_cooldown
    scale_out_cooldown = var.scale_out_cooldown

    predefined_metric_specification {
      predefined_metric_type = "ECSServiceAverageMemoryUtilization"
    }
  }
}
