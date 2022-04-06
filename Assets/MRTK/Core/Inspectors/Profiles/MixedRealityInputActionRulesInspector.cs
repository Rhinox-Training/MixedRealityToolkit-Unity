// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License. 

using System;
using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(MixedRealityInputActionRulesProfile))]
    public class MixedRealityInputActionRulesInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent RuleAddButtonContent = new GUIContent("+ Add a New Rule Definition");
        private static readonly GUIContent RuleMinusButtonContent = new GUIContent("-", "Remove Rule Definition");
        private static readonly GUIContent BaseActionContent = new GUIContent("Base Input Action:", "The Action that will raise new actions based on the criteria met");
        private static readonly GUIContent RuleActionContent = new GUIContent("Rule Input Action:", "The Action that will be raised when the criteria is met");
        private static readonly GUIContent CriteriaContent = new GUIContent("Action Criteria:", "The Criteria that must be met in order to raise the new Action");

        private const string ProfileTitle = "Input Action Rule Settings";
        private const string ProfileDescription = "Input Action Rules help define alternative Actions that will be raised based on specific criteria.\n\n" +
                                    "You can create new rules by assigning a base Input Action below, then assigning the criteria you'd like to meet. When the criteria is met, the Rule's Action will be raised with the criteria value.\n\n" +
                                    "Note: Rules can only be created for the same axis constraints.";

        private SerializedProperty inputActionRulesDigital;
        private SerializedProperty inputActionRulesSingleAxis;
        private SerializedProperty inputActionRulesDualAxis;
        private SerializedProperty inputActionRulesVectorAxis;
        private SerializedProperty inputActionRulesQuaternionAxis;
        private SerializedProperty inputActionRulesPoseAxis;

        private int[] baseActionIds = System.Array.Empty<int>();
        private string[] baseActionLabels = System.Array.Empty<string>();

        // These are marked as static because this inspector will reset itself every refresh
        // because it can be rendered as a sub-profile and thus OnEnable() is called every time
        private static int[] ruleActionIds = System.Array.Empty<int>();
        private static string[] ruleActionLabels = System.Array.Empty<string>();

        private static int selectedBaseActionId = 0;
        private static int selectedRuleActionId = 0;

        private static MixedRealityInputAction currentBaseAction = MixedRealityInputAction.None;
        private static MixedRealityInputAction currentRuleAction = MixedRealityInputAction.None;

        private static bool currentBoolCriteria;
        private static float currentSingleAxisCriteria;
        private static Vector2 currentDualAxisCriteria;
        private static Vector3 currentVectorCriteria;
        private static Quaternion currentQuaternionCriteria;
        private static MixedRealityPose currentPoseCriteria;
        
        private static bool currentBoolCriteriaToggle;
        private static bool currentSingleAxisCriteriaToggle;
        private static bool currentDualAxisCriteriaToggle;
        private static bool currentVectorCriteriaToggle;
        private static bool currentQuaternionCriteriaToggle;
        private static bool currentPoseCriteriaToggle;

        private static InputActionRuleCriteriaDigital currentDigitalRuleCriteria;
        private static InputActionRuleCriteriaSingleAxis currentSingleAxisRuleCriteria;
        private static InputActionRuleCriteriaDualAxis currentDualAxisRuleCriteria;
        private static InputActionRuleCriteriaVectorAxis currentVectorRuleCriteria;
        private static InputActionRuleCriteriaQuaternionAxis currentQuaternionRuleCriteria;
        private static InputActionRuleCriteriaPoseAxis currentPoseRuleCriteria;
        
        private static int[] comparisonIds = System.Array.Empty<int>();
        private static string[] comparisonLabels = System.Array.Empty<string>();

        private static bool[] digitalFoldouts;
        private static bool[] singleAxisFoldouts;
        private static bool[] dualAxisFoldouts;
        private static bool[] vectorFoldouts;
        private static bool[] quaternionFoldouts;
        private static bool[] poseFoldouts;

        private static GUIContent tempContent;

        private MixedRealityInputActionRulesProfile thisProfile;
        private bool isInitialized = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            isInitialized = false;

            inputActionRulesDigital = serializedObject.FindProperty("inputActionRulesDigital");
            inputActionRulesSingleAxis = serializedObject.FindProperty("inputActionRulesSingleAxis");
            inputActionRulesDualAxis = serializedObject.FindProperty("inputActionRulesDualAxis");
            inputActionRulesVectorAxis = serializedObject.FindProperty("inputActionRulesVectorAxis");
            inputActionRulesQuaternionAxis = serializedObject.FindProperty("inputActionRulesQuaternionAxis");
            inputActionRulesPoseAxis = serializedObject.FindProperty("inputActionRulesPoseAxis");

            comparisonIds = (int[]) Enum.GetValues(typeof(ActionRuleComparison));
            comparisonLabels = Enum.GetNames(typeof(ActionRuleComparison));
            
            tempContent = new GUIContent();

            thisProfile = target as MixedRealityInputActionRulesProfile;

            // Only reset if we haven't get done so
            if (digitalFoldouts == null || HasFoldoutCountChanged())
            {
                ResetCriteria();
            }

            if (!IsProfileInActiveInstance()
                || MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
            {
                return;
            }

            var inputActions = GetInputActions();
            baseActionLabels = inputActions.Where(action => action.AxisConstraint != AxisType.None && action.AxisConstraint != AxisType.Raw)
                                            .Select(action => action.Description).ToArray();

            baseActionIds = inputActions.Where(action => action.AxisConstraint != AxisType.None && action.AxisConstraint != AxisType.Raw)
                                        .Select(action => (int)action.Id).ToArray();

            isInitialized = true;
        }

        private bool HasFoldoutCountChanged()
        {
            if (digitalFoldouts != null && digitalFoldouts.Length != inputActionRulesDigital.arraySize)
            {
                return true;
            }

            if (singleAxisFoldouts != null && singleAxisFoldouts.Length != inputActionRulesSingleAxis.arraySize)
            {
                return true;
            }
            
            if (dualAxisFoldouts != null && dualAxisFoldouts.Length != inputActionRulesDualAxis.arraySize)
            {
                return true;
            }
            
            if (vectorFoldouts != null && vectorFoldouts.Length != inputActionRulesVectorAxis.arraySize)
            {
                return true;
            }
            
            if (quaternionFoldouts != null && quaternionFoldouts.Length != inputActionRulesQuaternionAxis.arraySize)
            {
                return true;
            }
            
            if (poseFoldouts != null && poseFoldouts.Length != inputActionRulesPoseAxis.arraySize)
            {
                return true;
            }

            return false;
        }

        public override void OnInspectorGUI()
        {
            if (!RenderProfileHeader(ProfileTitle, ProfileDescription, target, isInitialized, BackProfileType.Input))
            {
                return;
            }

            CheckMixedRealityInputActions();

            using (new EditorGUI.DisabledGroupScope(IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                selectedBaseActionId = RenderBaseInputAction(selectedBaseActionId, out currentBaseAction);

                using (new EditorGUI.DisabledGroupScope(currentBaseAction == MixedRealityInputAction.None))
                {
                    RenderCriteriaField(currentBaseAction);

                    if (selectedBaseActionId == selectedRuleActionId)
                    {
                        selectedRuleActionId = 0;
                    }

                    selectedRuleActionId = RenderRuleInputAction(selectedRuleActionId, out currentRuleAction);

                    EditorGUILayout.Space();
                }

                bool addButtonEnable = !RuleExists() &&
                          currentBaseAction != MixedRealityInputAction.None &&
                          currentRuleAction != MixedRealityInputAction.None &&
                          currentBaseAction.AxisConstraint != AxisType.None &&
                          currentBaseAction.AxisConstraint != AxisType.Raw;

                using (new EditorGUI.DisabledGroupScope(!addButtonEnable))
                {
                    if (InspectorUIUtility.RenderIndentedButton(RuleAddButtonContent, EditorStyles.miniButton))
                    {
                        AddRule();
                        ResetCriteria();
                    }
                }

                EditorGUILayout.Space();

                var isWideMode = EditorGUIUtility.wideMode;
                EditorGUIUtility.wideMode = true;

                RenderList(inputActionRulesDigital, digitalFoldouts);
                RenderList(inputActionRulesSingleAxis, singleAxisFoldouts);
                RenderList(inputActionRulesDualAxis, dualAxisFoldouts);
                RenderList(inputActionRulesVectorAxis, vectorFoldouts);
                RenderList(inputActionRulesQuaternionAxis, quaternionFoldouts);
                RenderList(inputActionRulesPoseAxis, poseFoldouts);

                EditorGUIUtility.wideMode = isWideMode;
                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile != null &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionRulesProfile;
        }

        private bool RuleExists()
        {
            switch (currentBaseAction.AxisConstraint)
            {
                default:
                    return false;
                case AxisType.Digital:
                    return thisProfile.InputActionRulesDigital.Any(digitalRule => digitalRule.BaseAction == currentBaseAction && digitalRule.RuleAction == currentRuleAction && digitalRule.Criteria == currentBoolCriteria);
                case AxisType.SingleAxis:
                    return thisProfile.InputActionRulesSingleAxis.Any(singleAxisRule => singleAxisRule.BaseAction == currentBaseAction && singleAxisRule.RuleAction == currentRuleAction && singleAxisRule.Criteria.Equals(currentSingleAxisCriteria));
                case AxisType.DualAxis:
                    return thisProfile.InputActionRulesDualAxis.Any(dualAxisRule => dualAxisRule.BaseAction == currentBaseAction && dualAxisRule.RuleAction == currentRuleAction && dualAxisRule.Criteria == currentDualAxisCriteria);
                case AxisType.ThreeDofPosition:
                    return thisProfile.InputActionRulesVectorAxis.Any(vectorAxisRule => vectorAxisRule.BaseAction == currentBaseAction && vectorAxisRule.RuleAction == currentRuleAction && vectorAxisRule.Criteria == currentVectorCriteria);
                case AxisType.ThreeDofRotation:
                    return thisProfile.InputActionRulesQuaternionAxis.Any(quaternionRule => quaternionRule.BaseAction == currentBaseAction && quaternionRule.RuleAction == currentRuleAction && quaternionRule.Criteria == currentQuaternionCriteria);
                case AxisType.SixDof:
                    return thisProfile.InputActionRulesPoseAxis.Any(poseRule => poseRule.BaseAction == currentBaseAction && poseRule.RuleAction == currentRuleAction && poseRule.Criteria == currentPoseCriteria);
            }
        }

        private void ResetCriteria()
        {
            selectedBaseActionId = 0;
            selectedRuleActionId = 0;
            currentBaseAction = MixedRealityInputAction.None;
            currentRuleAction = MixedRealityInputAction.None;
            currentBoolCriteria = false;
            currentSingleAxisCriteria = 0f;
            currentDualAxisCriteria = Vector2.zero;
            currentVectorCriteria = Vector3.zero;
            currentQuaternionCriteria = Quaternion.identity;
            currentPoseCriteria = MixedRealityPose.ZeroIdentity;
            
            currentBoolCriteriaToggle = false;
            currentSingleAxisCriteriaToggle = false;
            currentDualAxisCriteriaToggle = false;
            currentVectorCriteriaToggle = false;
            currentQuaternionCriteriaToggle = false;
            currentPoseCriteriaToggle = false;

            currentDigitalRuleCriteria = new InputActionRuleCriteriaDigital(false);
            currentSingleAxisRuleCriteria = new InputActionRuleCriteriaSingleAxis(0f);
            currentDualAxisRuleCriteria = new InputActionRuleCriteriaDualAxis(Vector2.zero);
            currentVectorRuleCriteria = new InputActionRuleCriteriaVectorAxis(Vector3.zero);
            currentQuaternionRuleCriteria = new InputActionRuleCriteriaQuaternionAxis(Quaternion.identity);
            currentPoseRuleCriteria = new InputActionRuleCriteriaPoseAxis(MixedRealityPose.ZeroIdentity);

            digitalFoldouts = new bool[inputActionRulesDigital.arraySize];
            singleAxisFoldouts = new bool[inputActionRulesSingleAxis.arraySize];
            dualAxisFoldouts = new bool[inputActionRulesDualAxis.arraySize];
            vectorFoldouts = new bool[inputActionRulesVectorAxis.arraySize];
            quaternionFoldouts = new bool[inputActionRulesQuaternionAxis.arraySize];
            poseFoldouts = new bool[inputActionRulesPoseAxis.arraySize];
        }

        private static void GetCompatibleActions(MixedRealityInputAction baseAction)
        {
            var inputActions = GetInputActions();

            ruleActionLabels = inputActions.Where(inputAction => CheckAxisConstraint(inputAction, baseAction.AxisConstraint) && inputAction.Id != baseAction.Id)
                .Select(action => action.Description).ToArray();

            ruleActionIds = inputActions.Where(inputAction => CheckAxisConstraint(inputAction, baseAction.AxisConstraint) && inputAction.Id != baseAction.Id)
                .Select(action => (int)action.Id).ToArray();
        }

        private static bool CheckAxisConstraint(MixedRealityInputAction inputAction, AxisType axisType)
        {
            if (inputAction.AxisConstraint == axisType)
            {
                return true;
            }

            if (axisType != AxisType.None &&
                axisType != AxisType.Raw &&
                axisType != AxisType.Digital)
            {
                return inputAction.AxisConstraint == AxisType.Digital || inputAction.AxisConstraint == AxisType.None;
            }
            return false;
        }

        private string GetToggleButtonText(bool state)
        {
            return state ? "Downgrade" : "Upgrade";
        }

        private void RenderCriteriaField(MixedRealityInputAction action, SerializedProperty criteriaValue = null, SerializedProperty criteriaRuleValue = null, SerializedProperty criteriaRuleComparison = null)
        {
            var isWideMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;
            if (action != MixedRealityInputAction.None)
            {
                switch (action.AxisConstraint)
                {
                    default:
                        EditorGUILayout.HelpBox("Base rule must have a valid axis constraint.", MessageType.Warning);
                        break;
                    case AxisType.Digital:
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            
                            EditorGUILayout.LabelField(CriteriaContent, GUILayout.Width(128));
                            //currentBoolCriteriaToggle = RenderCriteriaLabelWithButton(currentBoolCriteriaToggle);
                            if (!currentBoolCriteriaToggle)
                            {
                                EditorGUI.BeginChangeCheck();
                                var boolValue = EditorGUILayout.Toggle(GUIContent.none, criteriaValue?.boolValue ?? currentBoolCriteria, GUILayout.Width(64), GUILayout.ExpandWidth(true));

                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (criteriaValue != null)
                                    {
                                        criteriaValue.boolValue = boolValue;
                                    }
                                    else
                                    {
                                        currentBoolCriteria = boolValue;
                                    }
                                }
                            }
                            else
                            {
                                EditorGUI.BeginChangeCheck();
                                var boolValue = EditorGUILayout.Toggle(GUIContent.none, criteriaRuleValue?.boolValue ?? currentDigitalRuleCriteria.Criteria, GUILayout.Width(64), GUILayout.ExpandWidth(true));

                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (criteriaRuleValue != null)
                                    {
                                        criteriaRuleValue.boolValue = boolValue;
                                    }
                                    else
                                    {
                                        currentDigitalRuleCriteria.Criteria = boolValue;
                                    }
                                }
                            }
                        }
                        break;
                    case AxisType.SingleAxis:
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            currentSingleAxisCriteriaToggle = RenderCriteriaLabelWithButton(currentSingleAxisCriteriaToggle);
                            if (!currentSingleAxisCriteriaToggle)
                            {
                                currentSingleAxisCriteria = RenderAndSetField(GUIContent.none, criteriaValue, currentSingleAxisCriteria);
                            }
                            else
                            {
                                currentSingleAxisRuleCriteria.Criteria = RenderAndSetField(GUIContent.none, criteriaRuleValue, currentSingleAxisRuleCriteria.Criteria);
                                
                                EditorGUI.BeginChangeCheck();
                                var enumIndex = EditorGUILayout.IntPopup(criteriaRuleComparison?.enumValueIndex ?? (int) currentSingleAxisRuleCriteria.Comparison, comparisonLabels, comparisonIds, GUILayout.ExpandWidth(true));

                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (criteriaRuleComparison != null)
                                    {
                                        criteriaRuleComparison.enumValueIndex = enumIndex;
                                    }
                                    else
                                    {
                                        currentSingleAxisRuleCriteria.Comparison = (ActionRuleComparison) enumIndex;
                                    }
                                }
                            }   
                        }
                        break;
                    case AxisType.DualAxis:
                        currentDualAxisCriteriaToggle = RenderCriteriaLabelWithButton(currentDualAxisCriteriaToggle);
                        using (new EditorGUI.IndentLevelScope())
                        {
                            if (!currentDualAxisCriteriaToggle)
                            {
                                currentDualAxisCriteria = RenderAndSetField("Position", criteriaValue, currentDualAxisCriteria);
                            }
                            else
                            {
                                currentDualAxisRuleCriteria.Criteria = RenderAndSetField("Position", criteriaRuleValue, currentDualAxisRuleCriteria.Criteria);
                            }    
                        }
                        break;
                    case AxisType.ThreeDofPosition:
                        currentVectorCriteriaToggle = RenderCriteriaLabelWithButton(currentVectorCriteriaToggle);
                        using (new EditorGUI.IndentLevelScope())
                        {
                            if (!currentVectorCriteriaToggle)
                            {
                                currentVectorCriteria = RenderAndSetField("Position", criteriaValue, currentVectorCriteria);
                            }
                            else
                            {
                                currentVectorRuleCriteria.Criteria = RenderAndSetField("Position", criteriaRuleValue, currentVectorRuleCriteria.Criteria);
                            }   
                        }
                        break;
                    case AxisType.ThreeDofRotation:
                        currentQuaternionCriteriaToggle = RenderCriteriaLabelWithButton(currentQuaternionCriteriaToggle);
                        using (new EditorGUI.IndentLevelScope())
                        {
                            if (!currentQuaternionCriteriaToggle)
                            {
                                currentQuaternionCriteria = RenderAndSetField("Rotation", criteriaValue, currentQuaternionCriteria);
                            }
                            else
                            {
                                currentQuaternionRuleCriteria.Criteria = RenderAndSetField("Rotation", criteriaRuleValue, currentQuaternionRuleCriteria.Criteria);
                            }    
                        }
                        break;
                    case AxisType.SixDof:
                        currentPoseCriteriaToggle = RenderCriteriaLabelWithButton(currentPoseCriteriaToggle);
                        using (new EditorGUI.IndentLevelScope())
                        {
                            if (!currentPoseCriteriaToggle)
                            {
                                var posePosition = currentPoseCriteria.Position;
                                var poseRotation = currentPoseCriteria.Rotation;

                                if (criteriaValue != null)
                                {
                                    posePosition = criteriaValue.FindPropertyRelative("position").vector3Value;
                                    poseRotation = criteriaValue.FindPropertyRelative("rotation").quaternionValue;
                                }

                                EditorGUI.BeginChangeCheck();
                                posePosition = EditorGUILayout.Vector3Field("Position", posePosition);

                                poseRotation.eulerAngles = EditorGUILayout.Vector3Field("Rotation", poseRotation.eulerAngles);

                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (criteriaValue != null)
                                    {
                                        criteriaValue.FindPropertyRelative("position").vector3Value = posePosition;
                                        criteriaValue.FindPropertyRelative("rotation").quaternionValue = poseRotation;
                                    }
                                    else
                                    {
                                        currentPoseCriteria.Position = posePosition;
                                        currentPoseCriteria.Rotation = poseRotation;
                                    }
                                }
                            }
                            else
                            {
                                var posePosition = currentPoseRuleCriteria.Criteria.Position;
                                var poseRotation = currentPoseRuleCriteria.Criteria.Rotation;

                                if (criteriaRuleValue != null)
                                {
                                    posePosition = criteriaRuleValue.FindPropertyRelative("position").vector3Value;
                                    poseRotation = criteriaRuleValue.FindPropertyRelative("rotation").quaternionValue;
                                }

                                EditorGUI.BeginChangeCheck();
                                posePosition = EditorGUILayout.Vector3Field("Position", posePosition);

                                poseRotation.eulerAngles = EditorGUILayout.Vector3Field("Rotation", poseRotation.eulerAngles);

                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (criteriaRuleValue != null)
                                    {
                                        criteriaRuleValue.FindPropertyRelative("position").vector3Value = posePosition;
                                        criteriaRuleValue.FindPropertyRelative("rotation").quaternionValue = poseRotation;
                                    }
                                    else
                                    {
                                        var criteriaPoseStruct = currentPoseRuleCriteria.Criteria;
                                        criteriaPoseStruct.Position = posePosition;
                                        criteriaPoseStruct.Rotation = poseRotation;
                                        currentPoseRuleCriteria.Criteria = criteriaPoseStruct;
                                    }
                                }
                            }
                        }
                        break;
                }

                EditorGUIUtility.wideMode = isWideMode;
            }
        }

        private void RenderCriteriaRuleField(MixedRealityInputAction action, SerializedProperty useCriteriaRule = null, SerializedProperty criteriaRuleValue = null)
        {
            var isWideMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;
            if (action != MixedRealityInputAction.None)
            {
                switch (action.AxisConstraint)
                {
                    default:
                        EditorGUILayout.HelpBox("Base rule must have a valid axis constraint.", MessageType.Warning);
                        break;
                    case AxisType.Digital:
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.BeginFoldoutHeaderGroup()
                        }

                        break;
                }
            }

            EditorGUIUtility.wideMode = isWideMode;
        }

        private float RenderAndSetField(string label, SerializedProperty fieldProperty, float fallback)
        {
            tempContent.text = label;
            return RenderAndSetField(tempContent, fieldProperty, fallback);
        }

        private float RenderAndSetField(GUIContent label, SerializedProperty fieldProperty, float fallback)
        {
            EditorGUI.BeginChangeCheck();
            var floatValue = EditorGUILayout.FloatField(label, fieldProperty?.floatValue ?? fallback, GUILayout.Width(64), GUILayout.ExpandWidth(true));

            if (EditorGUI.EndChangeCheck())
            {
                if (fieldProperty != null)
                {
                    fieldProperty.floatValue = floatValue;
                }
                else
                {
                    fallback = floatValue;
                }
            }

            return fallback;
        }
        
        private Vector2 RenderAndSetField(string label, SerializedProperty fieldProperty, Vector2 fallback)
        {
            EditorGUI.BeginChangeCheck();
            var positionValue = EditorGUILayout.Vector2Field(label, fieldProperty?.vector2Value ?? fallback, GUILayout.ExpandWidth(true));
            
            if (EditorGUI.EndChangeCheck())
            {
                if (fieldProperty != null)
                {
                    fieldProperty.vector2Value = positionValue;
                }
                else
                {
                    fallback = positionValue;
                }
            }

            return fallback;
        }

        private Vector3 RenderAndSetField(string label, SerializedProperty fieldProperty, Vector3 fallback)
        {
            EditorGUI.BeginChangeCheck();
            var positionValue = EditorGUILayout.Vector3Field(label, fieldProperty?.vector3Value ?? fallback, GUILayout.ExpandWidth(true));
            
            if (EditorGUI.EndChangeCheck())
            {
                if (fieldProperty != null)
                {
                    fieldProperty.vector3Value = positionValue;
                }
                else
                {
                    fallback = positionValue;
                }
            }

            return fallback;
        }

        private Quaternion RenderAndSetField(string label, SerializedProperty fieldProperty, Quaternion fallback)
        {
            EditorGUI.BeginChangeCheck();
            var rotationValue = EditorGUILayout.Vector3Field(label, fieldProperty?.quaternionValue.eulerAngles ?? fallback.eulerAngles, GUILayout.ExpandWidth(true));

            if (EditorGUI.EndChangeCheck())
            {
                if (fieldProperty != null)
                {
                    fieldProperty.quaternionValue = Quaternion.Euler(rotationValue);
                }
                else
                {
                    fallback = Quaternion.Euler(rotationValue);
                }
            }

            return fallback;
        }

        private bool RenderCriteriaLabelWithButton(bool toggle)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(GetToggleButtonText(toggle), GUILayout.ExpandWidth(false)))
            {
                toggle = !toggle;
            }

            EditorGUILayout.LabelField(CriteriaContent, GUILayout.Width(128));
            EditorGUILayout.EndHorizontal();
            return toggle;
        }

        private void AddRule()
        {
            SerializedProperty rule;
            switch (currentBaseAction.AxisConstraint)
            {
                case AxisType.Digital:
                    inputActionRulesDigital.arraySize += 1;
                    rule = inputActionRulesDigital.GetArrayElementAtIndex(inputActionRulesDigital.arraySize - 1);
                    rule.FindPropertyRelative("criteria").boolValue = currentBoolCriteria;
                    var criteriaRuleDigital = rule.FindPropertyRelative("criteriaRule");
                    criteriaRuleDigital.FindPropertyRelative("criteria").boolValue = currentDigitalRuleCriteria.Criteria;
                    break;
                case AxisType.SingleAxis:
                    inputActionRulesSingleAxis.arraySize += 1;
                    rule = inputActionRulesSingleAxis.GetArrayElementAtIndex(inputActionRulesSingleAxis.arraySize - 1);
                    rule.FindPropertyRelative("criteria").floatValue = currentSingleAxisCriteria;
                    var criteriaRuleSingle = rule.FindPropertyRelative("criteriaRule");
                    criteriaRuleSingle.FindPropertyRelative("criteria").floatValue = currentSingleAxisRuleCriteria.Criteria;
                    break;
                case AxisType.DualAxis:
                    inputActionRulesDualAxis.arraySize += 1;
                    rule = inputActionRulesDualAxis.GetArrayElementAtIndex(inputActionRulesDualAxis.arraySize - 1);
                    rule.FindPropertyRelative("criteria").vector2Value = currentDualAxisCriteria;
                    break;
                case AxisType.ThreeDofPosition:
                    inputActionRulesVectorAxis.arraySize += 1;
                    rule = inputActionRulesVectorAxis.GetArrayElementAtIndex(inputActionRulesVectorAxis.arraySize - 1);
                    rule.FindPropertyRelative("criteria").vector3Value = currentVectorCriteria;
                    break;
                case AxisType.ThreeDofRotation:
                    inputActionRulesQuaternionAxis.arraySize += 1;
                    rule = inputActionRulesQuaternionAxis.GetArrayElementAtIndex(inputActionRulesQuaternionAxis.arraySize - 1);
                    rule.FindPropertyRelative("criteria").quaternionValue = currentQuaternionCriteria;
                    break;
                case AxisType.SixDof:
                    inputActionRulesPoseAxis.arraySize += 1;
                    rule = inputActionRulesPoseAxis.GetArrayElementAtIndex(inputActionRulesPoseAxis.arraySize - 1);
                    var criteria = rule.FindPropertyRelative("criteria");
                    criteria.FindPropertyRelative("position").vector3Value = currentPoseCriteria.Position;
                    criteria.FindPropertyRelative("rotation").quaternionValue = currentPoseCriteria.Rotation;
                    break;
                default:
                    Debug.LogError("Invalid Axis Constraint!");
                    return;
            }

            var baseAction = rule.FindPropertyRelative("baseAction");
            var baseActionId = baseAction.FindPropertyRelative("id");
            var baseActionDescription = baseAction.FindPropertyRelative("description");
            var baseActionConstraint = baseAction.FindPropertyRelative("axisConstraint");

            baseActionId.intValue = (int)currentBaseAction.Id;
            baseActionDescription.stringValue = currentBaseAction.Description;
            baseActionConstraint.intValue = (int)currentBaseAction.AxisConstraint;

            var ruleAction = rule.FindPropertyRelative("ruleAction");
            var ruleActionId = ruleAction.FindPropertyRelative("id");
            var ruleActionDescription = ruleAction.FindPropertyRelative("description");
            var ruleActionConstraint = ruleAction.FindPropertyRelative("axisConstraint");

            ruleActionId.intValue = (int)currentRuleAction.Id;
            ruleActionDescription.stringValue = currentRuleAction.Description;
            ruleActionConstraint.intValue = (int)currentRuleAction.AxisConstraint;
        }

        private int RenderBaseInputAction(int baseActionId, out MixedRealityInputAction action, bool isLocked = false)
        {
            using (new EditorGUI.DisabledGroupScope(!isInitialized))
            {
                action = MixedRealityInputAction.None;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(BaseActionContent);
                EditorGUI.BeginChangeCheck();

                if (!isLocked)
                {
                    baseActionId = EditorGUILayout.IntPopup(baseActionId, baseActionLabels, baseActionIds, GUILayout.ExpandWidth(true));
                }

                var inputActions = GetInputActions();
                for (int i = 0; i < inputActions.Length; i++)
                {
                    if (baseActionId == (int)inputActions[i].Id)
                    {
                        action = inputActions[i];
                    }
                }

                if (action != MixedRealityInputAction.None)
                {
                    GetCompatibleActions(action);
                }

                if (isLocked)
                {
                    EditorGUILayout.LabelField(action.Description, EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
                }

                EditorGUILayout.EndHorizontal();
            }

            return baseActionId;
        }

        private int RenderRuleInputAction(int ruleActionId, out MixedRealityInputAction action)
        {
            action = MixedRealityInputAction.None;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(RuleActionContent, GUILayout.Width(128));
            EditorGUI.BeginChangeCheck();
            ruleActionId = EditorGUILayout.IntPopup(ruleActionId, ruleActionLabels, ruleActionIds, GUILayout.ExpandWidth(true));

            var inputActions = GetInputActions();
            for (int i = 0; i < inputActions.Length; i++)
            {
                if (ruleActionId == (int)inputActions[i].Id)
                {
                    action = inputActions[i];
                }
            }

            EditorGUILayout.EndHorizontal();
            return ruleActionId;
        }

        private void RenderList(SerializedProperty list, bool[] foldouts)
        {
            for (int i = 0; i < list?.arraySize; i++)
            {
                var rule = list.GetArrayElementAtIndex(i);
                var criteria = rule.FindPropertyRelative("criteria");
                var useCriteriaRule = rule.FindPropertyRelative("useCriteriaRule");
                var criteriaRule = rule.FindPropertyRelative("criteriaRule");
                var criteriaRuleValue = criteriaRule.FindPropertyRelative("criteria");
                var criteriaRuleComparison = criteriaRule.FindPropertyRelative("comparison");

                var baseAction = rule.FindPropertyRelative("baseAction");
                var baseActionId = baseAction.FindPropertyRelative("id");
                var baseActionDescription = baseAction.FindPropertyRelative("description");
                var baseActionConstraint = baseAction.FindPropertyRelative("axisConstraint");

                var ruleAction = rule.FindPropertyRelative("ruleAction");
                var ruleActionId = ruleAction.FindPropertyRelative("id");
                var ruleActionDescription = ruleAction.FindPropertyRelative("description");
                var ruleActionConstraint = ruleAction.FindPropertyRelative("axisConstraint");

                using (new EditorGUILayout.HorizontalScope())
                {
                    foldouts[i] = EditorGUILayout.Foldout(foldouts[i], new GUIContent($"{baseActionDescription.stringValue} -> {ruleActionDescription.stringValue}"), true);

                    if (GUILayout.Button(RuleMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                    {
                        list.DeleteArrayElementAtIndex(i);
                        return;
                    }
                }

                if (foldouts[i])
                {
                    EditorGUI.indentLevel++;

                    MixedRealityInputAction newBaseAction;
                    baseActionId.intValue = RenderBaseInputAction(baseActionId.intValue, out newBaseAction, true);
                    baseActionDescription.stringValue = newBaseAction.Description;
                    baseActionConstraint.intValue = (int)newBaseAction.AxisConstraint;

                    if (baseActionId.intValue == ruleActionId.intValue || newBaseAction == MixedRealityInputAction.None || baseActionConstraint.intValue != ruleActionConstraint.intValue)
                    {
                        criteria.Reset();
                        criteriaRule.Reset();
                        ruleActionId.intValue = (int)MixedRealityInputAction.None.Id;
                        ruleActionDescription.stringValue = MixedRealityInputAction.None.Description;
                        ruleActionConstraint.intValue = (int)MixedRealityInputAction.None.AxisConstraint;
                    }
                    
                    EditorGUI.BeginDisabledGroup(useCriteriaRule.boolValue);
                    RenderCriteriaField(newBaseAction, criteria, criteriaRuleValue, criteriaRuleComparison);
                    EditorGUI.EndDisabledGroup();
                    RenderCriteriaRuleField(newBaseAction, criteriaRule);
                    
                    
                    MixedRealityInputAction newRuleAction;
                    ruleActionId.intValue = RenderRuleInputAction(ruleActionId.intValue, out newRuleAction);
                    ruleActionDescription.stringValue = newRuleAction.Description;
                    ruleActionConstraint.intValue = (int)newRuleAction.AxisConstraint;
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }
        }

        private static MixedRealityInputAction[] GetInputActions()
        {
            if (!MixedRealityToolkit.IsInitialized ||
                !MixedRealityToolkit.Instance.HasActiveProfile ||
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile == null ||
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile == null)
            {
                return System.Array.Empty<MixedRealityInputAction>();
            }

            return MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.InputActionsProfile.InputActions;
        }
    }
}