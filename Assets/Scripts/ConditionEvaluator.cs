using Assets.Scripts;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;

public class ConditionEvaluator {
    private static ExperimentObjectRegistry experimentObjectRegistry = ExperimentObjectRegistry.Instance;

    private static bool TryGetValue(When condition, out object value) {
        value = null;

        if (!experimentObjectRegistry.TryGetInteractable(condition.Identifier, out var obj))
            return false;
        if (!obj.TryGet(condition.Property, out value))
            return false;

        return true;
    }
    public static bool Check(List<When> conditions) {
        foreach (var c in conditions) {
            if (!TryGetValue(c, out var currentValue))
                return false;


            if (!Compare(currentValue, c.Value, c.Comparison))
                return false;
        }

        return true;
    }

    private static bool IsNumeric(object obj) {
        return obj is byte or sbyte
            or short or ushort
            or int or uint
            or long or ulong
            or float or double
            or decimal;
    }

    public static bool Compare(object a, object b, ComparisonType comparison, float tolerance = 0.01f) {
        if (a == null || b == null)
            return false;

        // NUMÉRICOS
        if (IsNumeric(a) && IsNumeric(b)) {
            double da = Convert.ToDouble(a);
            double db = Convert.ToDouble(b);

            return comparison switch {
                ComparisonType.EQUAL => Math.Abs(da - db) <= tolerance,
                ComparisonType.NOT_EQUAL => Math.Abs(da - db) > tolerance,
                ComparisonType.GREATER_THAN => da > db,
                ComparisonType.LESS_THAN => da < db,
                ComparisonType.GREATER_THAN_OR_EQUAL => da >= db,
                ComparisonType.LESS_THAN_OR_EQUAL => da <= db,
                _ => false
            };
        }

        //  BOOLEAN
        if (a is bool ba) {
            return comparison switch {
                ComparisonType.TRUE => ba,
                ComparisonType.FALSE => !ba,
                ComparisonType.EQUAL => ba.Equals(b),
                ComparisonType.NOT_EQUAL => !ba.Equals(b),
                _ => false
            };
        }

        //  STRING / DEFAULT
        return comparison switch {
            ComparisonType.EQUAL => a.Equals(b),
            ComparisonType.NOT_EQUAL => !a.Equals(b),
            _ => false
        };
    }
}