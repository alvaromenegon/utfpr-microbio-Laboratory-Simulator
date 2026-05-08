using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ConditionEvaluator {
    private static ExperimentObjectRegistry experimentObjectRegistry = ExperimentObjectRegistry.Instance;

    private static object GetDefaultValue(When condition) {
        return condition.Comparison switch {
            ComparisonType.TRUE => false,
            ComparisonType.FALSE => false,

            ComparisonType.EQUAL => null,
            ComparisonType.NOT_EQUAL => null,

            _ => null
        };
    }

    private static bool TryGetValue(When condition, out object value) {
        value = null;
        //Debug.Log($"Trying to get value for condition: {condition.Identifier}.{condition.Property}");

        if (!experimentObjectRegistry.TryGetObject(condition.Identifier, out var obj)) {
            //Debug.LogWarning($"Object with identifier '{condition.Identifier}' not found.");
            return false;
        }
        if (!obj.TryGetProperty(condition.Property, out value)) {
            value = GetDefaultValue(condition);
        }
        //Debug.Log($"Retrieved value for {condition.Identifier}.{condition.Property}: {value}");
        return true;
    }
    public static bool Check(List<When> conditions) {
        foreach (var c in conditions) {
            if (!TryGetValue(c, out var currentValue))
                return false;


            if (!Compare(currentValue, c.Value, c.Comparison)) {
                //Debug.Log($"Condition failed: {c.Identifier}.{c.Property} {c.Comparison} {c.Value ?? "null"} (current value: {currentValue})");
                return false;
             }
        }
        //Debug.Log("All conditions passed.");

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
       if ( !(comparison == ComparisonType.TRUE || comparison == ComparisonType.FALSE) && (a == null || b == null)) {
            //Debug.LogWarning($"Cannot perform comparison '{comparison}' when one of the values is null: a={a}, b={b}");
            return false;
        }

        // NUMÉRICOS
        if (IsNumeric(a) && IsNumeric(b)) {
            double da = Convert.ToDouble(a);
            double db = Convert.ToDouble(b);
            //Debug.Log($"Comparing numeric values: {da} {comparison} {db} with tolerance {tolerance}");

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

        //Debug.Log($"Comparing values: {a} {comparison} {b} (type: {a.GetType()})");

        //  STRING / DEFAULT
        return comparison switch {
            ComparisonType.EQUAL => a.Equals(b),
            ComparisonType.NOT_EQUAL => !a.Equals(b),
            _ => false
        };
    }
}