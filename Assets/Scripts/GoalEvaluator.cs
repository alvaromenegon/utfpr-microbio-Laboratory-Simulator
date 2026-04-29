using System;

namespace Assets.Scripts {
    public class GoalEvaluator {

        private readonly ExperimentObjectRegistry registry;

        // se o valor for um número, considerar a tolerância
        // por isso, não chamamos o Equals diretamente no Evaluate(), mas sim um método separado
        private bool IsEqual(object a, object b, float tolerance = 0.001f) {
            if (a is double da && b is double db) {
                return Math.Abs(da - db) <= tolerance;
            }
            return Equals(a, b);
        }

        public bool Evaluate(StepGoal goal) {
            registry.TryGetInteractable(goal.Identifier, out var obj);
            var current = obj.Get<object>(goal.Property);
            return goal.Comparison switch {
                ComparisonType.EQUAL => IsEqual(current, goal.Value, goal.Tolerance),
                ComparisonType.NOT_EQUAL => !IsEqual(current, goal.Value, goal.Tolerance),
                ComparisonType.GREATER_THAN => (Convert.ToDouble(current) > Convert.ToDouble(goal.Value) + goal.Tolerance),
                ComparisonType.LESS_THAN => Convert.ToDouble(current) < Convert.ToDouble(goal.Value) - goal.Tolerance,
                ComparisonType.GREATER_THAN_OR_EQUAL => Convert.ToDouble(current) >= Convert.ToDouble(goal.Value) - goal.Tolerance,
                ComparisonType.LESS_THAN_OR_EQUAL => Convert.ToDouble(current) <= Convert.ToDouble(goal.Value) + goal.Tolerance ,
                ComparisonType.TRUE => (bool)current,
                ComparisonType.FALSE => !(bool)current,
                _ => false,
            };
        }
    }
}