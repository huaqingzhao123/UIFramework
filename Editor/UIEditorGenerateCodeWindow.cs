using System.Collections.Generic;
using UnityEngine;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using Sirenix.Utilities;
using Sirenix.OdinInspector;
using System.IO;
using UnityEditor;
using System.Reflection;
using WingjoyFramework.UIFramework.Runtime.Views;
using WingjoyFramework.UIFramework.Runtime.Utility;

namespace WingjoyFramework.UIFramework.Editor
{
    public class UIEditorGenerateCodeWindow : GlobalConfig<UIEditorGenerateCodeWindow>
    {
        /// <summary>
        /// 窗体数据        /// </summary>
        public List<UIFormData> UiFormDataList;

        [Serializable]
        public class UIFormData
        {
            /// <summary>
            /// 窗体
            /// </summary>
            [HorizontalGroup("Meta", Width = 200), HideLabel]
            public GameObject UiFormPrefab;

            /// <summary>
            /// 窗体脚本
            /// </summary>
            [HorizontalGroup("Meta", Width = 200), HideLabel]
            public TextAsset UiFormScript;


            /// <summary>
            /// 创建脚本
            /// </summary>
            [HorizontalGroup("Meta"), Button(Name = "Script")]
            public void CreateScript()
            {
                if (UiFormPrefab != null)
                {
                    string folder = "Assets/Script/UIForm";
                    string fieldScript = $"{folder}/{UiFormPrefab.name}.cs";
                    var directoryName = Path.GetDirectoryName(fieldScript);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    string logicScript = $"{folder}/{UiFormPrefab.name}Logic.cs";

                    CodeCompileUnit unit = new CodeCompileUnit();
                    CodeNamespace sampleNamespace = new CodeNamespace("Views");
                    sampleNamespace.Imports.Add(new CodeNamespaceImport("System"));
                    CodeTypeDeclaration logicClass = new CodeTypeDeclaration(UiFormPrefab.name);
                    logicClass.BaseTypes.Add(new CodeTypeReference(typeof(BaseView)));
                    logicClass.TypeAttributes = TypeAttributes.Public;
                    logicClass.IsClass = true;
                    logicClass.IsPartial = true;

                    sampleNamespace.Types.Add(logicClass);
                    unit.Namespaces.Add(sampleNamespace);

                    //生成代码
                    CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                    if (!File.Exists(logicScript))
                    {
                        // var methodInfos = typeof(UIFormBase).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                        // foreach (var methodInfo in methodInfos)
                        // {
                        //     if (methodInfo.IsVirtual)
                        //     {
                        //         CodeMemberMethod overrideMethod = new CodeMemberMethod();
                        //         overrideMethod.Name = methodInfo.Name;
                        //         overrideMethod.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                        //         overrideMethod.ReturnType = new CodeTypeReference(methodInfo.ReturnType);
                        //
                        //         foreach (var parameterInfo in methodInfo.GetParameters())
                        //         {
                        //             overrideMethod.Parameters.Add(new CodeParameterDeclarationExpression(parameterInfo.ParameterType, parameterInfo.Name));
                        //         }
                        //
                        //         logicClass.Members.Add(overrideMethod);
                        //     }    
                        // }
                        //
                        //
                        CodeGeneratorOptions autoScriptOptions = new CodeGeneratorOptions();
                        autoScriptOptions.BracingStyle = "C";
                        autoScriptOptions.BlankLinesBetweenMembers = true;
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(logicScript))
                        {
                            provider.GenerateCodeFromCompileUnit(unit, sw, autoScriptOptions);
                        }
                    }

                    CodeGeneratorOptions logicScriptOptions = new CodeGeneratorOptions();
                    logicScriptOptions.BracingStyle = "C";
                    logicClass.Comments.Add(new CodeCommentStatement("当前文件自动生成，禁止修改", true));
                    var uiFields = UiFormPrefab.GetComponentsInChildren<ViewField>();
                    foreach (var uiField in uiFields)
                    {
                        foreach (var component in uiField.Components)
                        {
                            //添加字段
                            var name = $"{component.name}{component.GetType().GetNiceName()}";
                            CodeMemberField field = new CodeMemberField(component.GetType(), $"m_{name}");
                            field.Comments.Add(new CodeCommentStatement(UIPathFind.GetPathWithoutCanvasEnvironment(component.transform), true));
                            field.Attributes = MemberAttributes.Private;
                            field.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializeField))));
                            logicClass.Members.Add(field);
                        }
                    }

                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fieldScript))
                    {
                        provider.GenerateCodeFromCompileUnit(unit, sw, logicScriptOptions);
                    }

                    AssetDatabase.Refresh();

                    UiFormScript = AssetDatabase.LoadAssetAtPath<TextAsset>(fieldScript);
                }
            }

            /// <summary>
            /// 实装字段
            /// </summary>
            [HorizontalGroup("Meta"), Button(Name = "Install"), ShowIf("@UiFormScript!=null")]
            public void Install()
            {
                if (UiFormPrefab != null && UiFormScript != null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (assembly.GetName().Name == "Assembly-CSharp")
                        {
                            var componentType = assembly.GetType($"{"Views"}.{UiFormPrefab.name}");
                            Component addComponent = UiFormPrefab.GetComponent(componentType);
                            if (addComponent == null)
                            {
                                addComponent = UiFormPrefab.AddComponent(componentType);
                            }

                            var uiFields = UiFormPrefab.GetComponentsInChildren<ViewField>();
                            foreach (var uiField in uiFields)
                            {
                                foreach (var component in uiField.Components)
                                {
                                    //实装字段
                                    var fieldName = $"m_{component.name}{component.GetType().GetNiceName()}";
                                    var fieldInfo = componentType.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                                    fieldInfo.SetValue(addComponent, component);
                                }
                            }
                        }
                    }

                    EditorUtility.SetDirty(UiFormPrefab);
                    AssetDatabase.SaveAssets();
                }
            }
        }
    }

}

