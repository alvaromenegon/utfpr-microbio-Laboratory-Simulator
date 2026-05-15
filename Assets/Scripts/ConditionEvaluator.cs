using Assets.Scripts;
using System;
using System.Collections.Generic;

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

    private static List<LaboratoryObject> ResolveObjects(ObjectReference reference, InteractionContext ctx) { 
        var resolvedObjects = new List<LaboratoryObject>();
        if (reference.Role.HasValue) {
            if (ctx.Roles.TryGetValue(reference.Role.Value, out var obj)) {
                resolvedObjects.Add(obj);
            }
        } else if (!string.IsNullOrEmpty(reference.InstanceId)) {
            if (experimentObjectRegistry.TryGetObjectInstance(reference.InstanceId, out var obj)) {
                resolvedObjects.Add(obj);
            }
        } else if (!string.IsNullOrEmpty(reference.CanonicalIdentifier)) {
            if (experimentObjectRegistry.TryGetObject(reference.CanonicalIdentifier, out var obj)) {
                resolvedObjects.Add(obj);
            }
        }
        return resolvedObjects;
    }

    private static bool TryGetValue(When condition, out object value) {
        value = null;
        //Debug.Log($"Trying to get value for condition: {condition.Identifier}.{condition.Property}");

        LaboratoryObject obj = null;
        
        // Try to get by InstanceId first (for multi-instance support)
        if (!experimentObjectRegistry.TryGetObjectInstance(condition.Target.CanonicalIdentifier, out obj)) {
            // Fall back to Identifier
            if (!experimentObjectRegistry.TryGetObject(condition.Target.CanonicalIdentifier, out obj)) {
                return false;
            }
        }
        
        if (!obj.TryGetProperty(condition.Property, out value)) {
            value = GetDefaultValue(condition);
        }
        //Debug.Log($"Retrieved value for {condition.Identifier}.{condition.Property}: {value}");
        return true;
    }
    public static bool Check(List<When> conditions, InteractionContext ctx = null) {
        foreach (var c in conditions) {
            if (c.Target.Role == null) {
                experimentObjectRegistry.TryGetObject(c.Target.CanonicalIdentifier, out var obj);
                if (obj == null) {
                    return false;
                }
                if (!obj.TryGetProperty(c.Property, value: out object value)) {
                    value = GetDefaultValue(c);
                }
                return Compare(value, c.Value, c.Comparison);


            }
            var objects = ResolveObjects(c.Target, ctx);
            if (objects.Count == 0) {
                return false;
            }
            if (c.MatchMode == QueryMatchMode.ANY) {
                bool anyMatch = false;
                foreach (var obj in objects) {
                    if (obj.TryGetProperty(c.Property, value: out object value)) {
                        if (Compare(value, c.Value, c.Comparison)) {
                            anyMatch = true;
                            break;
                        }
                    }
                    else {
                        if (Compare(GetDefaultValue(c), c.Value, c.Comparison)) {
                            anyMatch = true;
                            break;
                        }
                    }
                }
                if (!anyMatch) {
                    return false;
                }
            }
            else if (c.MatchMode == QueryMatchMode.ALL) {
                foreach (var obj in objects) {
                    if (obj.TryGetProperty(c.Property, value: out object value)) {
                        if (!Compare(value, c.Value, c.Comparison)) {
                            return false;
                        }
                    }
                    else {
                        if (!Compare(GetDefaultValue(c), c.Value, c.Comparison)) {
                            return false;
                        }
                    }
                }
            }
            else if (c.MatchMode == QueryMatchMode.NONE) {
                foreach (var obj in objects) {
                    if (obj.TryGetProperty(c.Property, value: out object value)) {
                        if (Compare(value, c.Value, c.Comparison)) {
                            return false;
                        }
                    }
                    else {
                        if (Compare(GetDefaultValue(c), c.Value, c.Comparison)) {
                            return false;
                        }
                    }
                }
            }else if (c.MatchMode == QueryMatchMode.EXACT) {
                if (objects.Count != 1) {
                    return false;
                }
                var obj = objects[0];
                if (obj.TryGetProperty(c.Property, value: out object value)) {
                    if (!Compare(value, c.Value, c.Comparison)) {
                        return false;
                    }
                }
                else {
                    if (!Compare(GetDefaultValue(c), c.Value, c.Comparison)) {
                        return false;
                    }
                }
            } else {
                //Debug.LogWarning($"Unsupported MatchMode '{c.MatchMode}' for condition: {c}");
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