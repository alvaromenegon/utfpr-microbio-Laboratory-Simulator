using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

public static class ExperimentMapper {
    public static ExperimentDefinition ToModel(ExperimentDefinitionDto dto) {
        if (dto == null) {
            return null;
        }

        return new ExperimentDefinition {
            id = dto.Id,
            title = dto.Title,
            createdAt = dto.CreatedAt,
            updatedAt = dto.UpdatedAt,
            userId = dto.UserId,
            experimentData = ToModel(dto.ExperimentData)
        };
    }

    public static ExperimentData ToModel(ExperimentDataDto dto) {
        if (dto == null) {
            return null;
        }

        return new ExperimentData {
            Description = dto.Description,
            Time = dto.Time,
            Objects = dto.Objects?.Select(ToModel).ToList() ?? new List<ExperimentInstanceObject>(),
            Steps = dto.Steps?.Select(ToModel).ToList() ?? new List<Step>(),
            BackgroundProcesses = dto.BackgroundProcesses?.Select(ToModel).ToList() ?? new List<BackgroundProcess>(),
            Effects = dto.Effects?.Select(ToModel).ToList() ?? new List<EffectDefinition>(),
            ReactiveEffects = dto.ReactiveEffects?.Select(ToModel).ToList() ?? new List<ReactiveEffect>(),
            ActionEffects = dto.ActionEffects?.Select(ToModel).ToList() ?? new List<ActionEffect>(),
            VisualAssets = dto.VisualAssets?.Select(ToModel).ToList() ?? new List<VisualAsset>(),
            VisualStateRules = dto.VisualStateRules?.Select(ToModel).ToList() ?? new List<VisualStateRule>()
        };
    }

    public static VisualStateRule ToModel(VisualStateRuleDto dto) {
        if (dto == null) {
            return null;
        }
        return new VisualStateRule {
            Id = dto.Id,
            ActionType= dto.ActionType,
            Subject = ToModel(dto.Subject),
            Conditions = dto.Conditions?.Select(ToModel).ToList() ?? new List<Condition>(),
             Value = dto.Value,
             Slot = dto.Slot,
             Priority = dto.Priority,
             VisualAssetId = dto.VisualAssetId,
             AnimatorParameter = dto.AnimatorParameter,
             AnimatorValue = dto.AnimatorValue,
        };
    }

    public static ExperimentInstanceObject ToModel(ExperimentObjectDto dto) {
        if (dto == null) {
            return null;
        }
        return new ExperimentInstanceObject {
            CanonicalIdentifier = dto.CanonicalIdentifier,
            InstanceId = dto.InstanceId,
            Tags = dto.Tags?.ToList() ?? new List<string>(),
            Binding = ToModel(dto.Binding),
            DisplayName = dto.DisplayName,
            InitialProperties = dto.InitialProperties ?? new Dictionary<string, object>()
        };
    }

    public static ObjectBinding ToModel(ObjectBindingDto dto) {
        if (dto == null) {
            return null;
        }
        return new ObjectBinding {
            Type = Enum.TryParse(dto.Type, out ObjectBindingType result) ? result : ObjectBindingType.SCENE,
            SceneObjectId = dto.SceneObjectId,
            SpawnLocation = dto.SpawnLocation
        };
    }

    public static Step ToModel(StepDto dto) {
        if (dto == null) {
            return null;
        }

        return new Step {
            Description = dto.Description,
            Score = dto.Score,
            Instruction = dto.Instruction,
            OnSuccessMessage = dto.OnSuccessMessage,
            OnFailureMessage = dto.OnFailureMessage,
            ActionOnFailure = dto.ActionOnFailure,
            OnFailureReturnToStep = dto.OnFailureReturnToStep,
            ActionOnSuccess = dto.ActionOnSuccess,
            Goals = dto.Goals?.Select(ToModel).ToList() ?? new List<Condition>(),
            FailureConditions = dto.FailureConditions?.Select(ToModel).ToList() ?? new List<Condition>(),
        };
    }

    public static BackgroundProcess ToModel(BackgroundProcessDto dto) {
        if (dto == null) {
            return null;
        }

        return new BackgroundProcess {
            Id = dto.Id,
            Description = dto.Description,
            Duration = dto.Duration,
            Tolerance = dto.Tolerance,
            OnSuccessMessage = dto.OnSuccessMessage,
            OnFailureMessage = dto.OnFailureMessage,
            ActionOnFailure = dto.ActionOnFailure,
            StartConditions = dto.StartConditions?.Select(ToModel).ToList() ?? new List<Condition>(),
            CancelConditions = dto.CancelConditions?.Select(ToModel).ToList() ?? new List<Condition>(),
            SuccessConditions = dto.SuccessConditions?.Select(ToModel).ToList() ?? new List<Condition>(),
            OnStartEffectIds = dto.OnStartEffectIds?.Select(id => id).ToList() ?? new List<string>(),
            OnSuccessEffectIds = dto.OnSuccessEffectIds?.Select(id => id).ToList() ?? new List<string>(),
            OnFailureEffectIds = dto.OnFailureEffectIds?.Select(id => id).ToList() ?? new List<string>(),
            BoundObject = ToModel(dto.BoundObject)
        };
    }

    public static Condition ToModel(WhenDto dto) {
        if (dto == null) {
            return null;
        }
        return new Condition {
            Target = ToModel(dto.Target),
            Property = dto.Property,
            Value = dto.Value,
            Comparison = Enum.TryParse(dto.Comparison, out ComparisonType result) ? result : ComparisonType.EQUAL,
            MatchMode = Enum.TryParse(dto.MatchMode, out QueryMatchMode matchResult) ? matchResult : QueryMatchMode.ALL
        };
    }

    public static EffectOperation ToModel(EffectOperationDto dto) {
        if (dto == null) {
            return null;
        }
        var op = new EffectOperation {
            Target = ToModel(dto.Target),
            Property = dto.Property,
            Value = dto.Value,
            OperationType = Enum.TryParse(dto.OperationType, out EffectOperationType result) ? result : EffectOperationType.SET_PROPERTY,
            ProcessId = dto.ProcessId

        };
        if (!string.IsNullOrEmpty(dto.DisplayMessageTemplate)) {
            op.SetDisplayMessageTemplate(dto.DisplayMessageTemplate);
        }
        return op;
    }

    public static ObjectReference ToModel(ObjectReferenceDto dto) {
        if (dto == null) {
            return null;
        }
        return new ObjectReference {
            InstanceId = dto.InstanceId,
            CanonicalIdentifier = dto.CanonicalIdentifier,
            Role = Enum.TryParse(dto.Role, out RoleType result) ? result : (RoleType?)null,
            RequiredTags = dto.RequiredTags?.ToList() ?? new List<string>(),
            ExcludedTags = dto.ExcludedTags?.ToList() ?? new List<string>(),
            RequiredZoneIds = dto.RequiredZoneIds?.ToList() ?? new List<string>(),
            RequiredZoneTags = dto.RequiredZoneTags?.ToList() ?? new List<string>(),
            ExcludedZoneTags = dto.ExcludedZoneTags?.ToList() ?? new List<string>()
        };
    }

    public static ReactiveEffect ToModel(ReactiveEffectDto dto) {
        if (dto == null) {
            return null;
        }
        return new ReactiveEffect {
            Id = dto.Id,
            DisplayName = dto.DisplayName,
            Priority = dto.Priority,
            Conditions = dto.Conditions?.Select(ToModel).ToList() ?? new List<Condition>(),
            Operations = dto.Operations?.Select(ToModel).ToList() ?? new List<EffectOperation>(),
            ExecuteOncePerSubject = dto.ExecuteOncePerSubject,
            Subject = ToModel(dto.Subject)
        };
    }

    public static ActionEffect ToModel(ActionEffectDto dto) {
        if (dto == null) {
            return null;
        }
        return new ActionEffect {
            Id = dto.Id,
            DisplayName = dto.DisplayName,
            Priority = dto.Priority,
            Operations = dto.Operations?.Select(ToModel).ToList() ?? new List<EffectOperation>()
        };
    }

    public static EffectDefinition ToModel(EffectDefinitionDto dto) {
        if (dto == null) {
            return null;
        }
        if (!Enum.TryParse(dto.ExecutionType, out EffectExecutionType executionType)) {
            throw new ArgumentException($"Invalid execution type: {dto.ExecutionType}");
        }

        return new EffectDefinition {
            Id = dto.Id,
            DisplayName = dto.DisplayName,
            Priority = dto.Priority,
            Type = Enum.TryParse(dto.Type, out Interaction.InteractionType result) ? result : Interaction.InteractionType.USE,
            ExecutionType = executionType,
            Property = dto.Property,
            Rate = dto.Rate,
            Operations = dto.Operations?.Select(ToModel).ToList() ?? new List<EffectOperation>(),
            Roles = dto.Roles?.ToDictionary(kv => ResolveRoleType(kv.Key), kv => kv.Value) ?? new Dictionary<RoleType, string>(),
            Conditions = dto.Conditions?.Select(ToModel).ToList() ?? new List<Condition>()
        };
    }

    public static VisualAsset ToModel( VisualAssetDto dto) {
        if (dto == null) {
            return null;
        }
        return new VisualAsset {
            Id = dto.Id,
            Type = dto.Type,
            AssetPath = dto.AssetPath,
            HideWhenInactive = dto.HideWhenInactive,
            ShaderTemplate = dto.ShaderTemplate           
        };
    }

    public static RoleType ResolveRoleType(string role) {
        return Enum.TryParse(role, out RoleType result) ? result : RoleType.ACTOR;
    }
}
