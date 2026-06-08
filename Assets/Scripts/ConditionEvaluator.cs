using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConditionEvaluator {
    private static object GetDefaultValue(Condition condition) {
        return condition.Comparison switch {
            ComparisonType.TRUE => false,
            ComparisonType.FALSE => false,

            ComparisonType.EQUAL => null,
            ComparisonType.NOT_EQUAL => null,

            _ => null
        };
    }

    public static bool Check(List<Condition> conditions, InteractionContext ctx = null) {
        foreach (var c in conditions) {           
            var resolver = new ObjectResolver(c.Target, ctx);
            

            var objects = resolver.Resolve();
            if (objects == null || objects.Count == 0) {
                return c.MatchMode == QueryMatchMode.NONE;
            }
            bool passed = c.MatchMode switch {
                QueryMatchMode.ANY => objects.Any(obj => Evaluate(obj, c)),

                QueryMatchMode.ALL => objects.All(obj => Evaluate(obj, c)),

                QueryMatchMode.NONE => !objects.Any(obj => Evaluate(obj, c)),

                QueryMatchMode.EXACT => objects.Count == 1 && objects.All(obj => Evaluate(obj, c)),
                _ => false
            };
            
            if (!passed)
                return false;           
        }

        return true;
    }

    private static bool Evaluate(LaboratoryObject obj, Condition c) {
        if (obj == null) return false;
        if (!obj.TryGetProperty<object>(c.Property, out var value)) {
            value = GetDefaultValue(c);
        }

        return Compare(value,c.Value,c.Comparison);
    }
                              
    public static bool Compare(object a, object b, ComparisonType comparison, float tolerance = 0.01f) {
       if ( !(comparison == ComparisonType.TRUE || comparison == ComparisonType.FALSE) && (a == null || b == null)) {
            //Debug.LogWarning($"Cannot perform comparison '{comparison}' when one of the values is null: a={a}, b={b}");
            return false;
        }

        // NUMÉRICOS
        if (Helpers.IsNumeric(a) && Helpers.IsNumeric(b)) {
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