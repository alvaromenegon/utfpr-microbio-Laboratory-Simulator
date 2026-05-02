using Assets.Scripts;
using System.Collections.Generic;

public class ConditionEvaluator {
    private static ExperimentObjectRegistry experimentObjectRegistry = ExperimentObjectRegistry.Instance;
    public static bool Check(List<When> conditions) {
        foreach (var c in conditions) {
            var obj = experimentObjectRegistry.TryGetInteractable(c.Identifier, out var interactable) ? interactable : null;
            var value = obj.Get<object>(c.Property);

            if (!Compare(value, c.Value, c.Comparison))
                return false;
        }
        return true;
    }

    public static bool Compare(object value1, object value2, ComparisonType comparison) {
        return comparison switch {
            ComparisonType.EQUAL => value1.Equals(value2),
            ComparisonType.NOT_EQUAL => !value1.Equals(value2),
            ComparisonType.GREATER_THAN => (bool)((dynamic)value1 > (dynamic)value2),
            ComparisonType.LESS_THAN => (bool)((dynamic)value1 < (dynamic)value2),
            ComparisonType.GREATER_THAN_OR_EQUAL => (bool)((dynamic)value1 >= (dynamic)value2),
            ComparisonType.LESS_THAN_OR_EQUAL => (bool)((dynamic)value1 <= (dynamic)value2),
            ComparisonType.TRUE => value1 is bool b1 && b1,
            ComparisonType.FALSE => value1 is bool b2 && !b2,
            _ => false,
        };
    }
}