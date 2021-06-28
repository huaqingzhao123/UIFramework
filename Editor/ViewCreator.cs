using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using WingjoyFramework.UIFramework.Runtime.Configs;
using WingjoyFramework.UIFramework.Runtime.Enums;
using WingjoyFramework.UIFramework.Runtime.Utility;
using WingjoyFramework.UIFramework.Runtime.Views;

namespace WingjoyFramework.UIFramework.Editor
{
    public class ViewCreator : GlobalConfig<ViewCreator>
    {
        /// <summary>
        /// 窗体数据
        /// </summary>
        public List<ViewData> ViewsDataList;

        [Serializable]
        public class ViewData
        {
            /// <summary>
            /// 窗体
            /// </summary>
            [HorizontalGroup("Meta", Width = 200), HideLabel]
            public GameObject ViewPrefab;

            /// <summary>
            /// 窗体脚本
            /// </summary>
            [HorizontalGroup("Meta", Width = 200), HideLabel]
            public TextAsset ViewScript;


            /// <summary>
            /// 创建脚本
            /// </summary>
            [HorizontalGroup("Meta"), Button(Name = "Script")]
            public void CreateScript()
            {
                if (ViewPrefab != null)
                {
                    string folder = "Assets/WingjoyFramework/UIFramework/Runtime/Views";
                    string fieldScript = $"{folder}/{ViewPrefab.name}/{ViewPrefab.name}.cs";
                    var rootDirectoryName = Path.GetDirectoryName(fieldScript);
                    if (!Directory.Exists(rootDirectoryName))
                    {
                        Directory.CreateDirectory(rootDirectoryName);
                    }
                    if (!Directory.Exists($"{folder}/{ViewPrefab.name}"))
                    {
                        Directory.CreateDirectory($"{folder}/{ViewPrefab.name}");
                    }
                    DirectoryInfo root = new DirectoryInfo(folder);
                    var directoriesInfo = root.GetDirectories();
                    CodeCompileUnit unit = new CodeCompileUnit();
                    CodeNamespace viewsNamespace = new CodeNamespace("WingjoyFramework.UIFramework.Runtime.Views");
                    viewsNamespace.Imports.Add(new CodeNamespaceImport("System"));
                    CodeTypeDeclaration viewClass = new CodeTypeDeclaration(ViewPrefab.name);
                    viewClass.BaseTypes.Add(new CodeTypeReference(typeof(BaseView)));
                    viewClass.TypeAttributes = TypeAttributes.Public;
                    viewClass.IsClass = true;
                    viewClass.IsPartial = true;
                    //覆盖UIViewType
                    CodeNamespace enumNamespace = new CodeNamespace("WingjoyFramework.UIFramework.Runtime.Enums");
                    CodeTypeDeclaration UIViewType = new CodeTypeDeclaration("UIViewType");
                    UIViewType.Comments.Add(new CodeCommentStatement("当前文件自动生成，禁止修改", true));
                    UIViewType.IsEnum = true;
                    UIViewType.Attributes = MemberAttributes.Public;
                    enumNamespace.Imports.Add(new CodeNamespaceImport("System"));
                    enumNamespace.Types.Add(UIViewType);
                    foreach (var item in directoriesInfo)
                    {
                        //添加枚举类型
                        CodeMemberField m = new CodeMemberField();
                        m.Name = item.Name;
                        UIViewType.Members.Add(m);
                    }


                    //生成代码
                    CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                    //写入配置
                    CodeGeneratorOptions logicScriptOptions = new CodeGeneratorOptions();
                    logicScriptOptions.BracingStyle = "C";
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter("Assets/WingjoyFramework/UIFramework/Runtime/Enums/UIViewType.cs"))
                    {
                        provider.GenerateCodeFromNamespace(enumNamespace, sw, logicScriptOptions);
                    }

                    //生成View脚本
                    viewsNamespace.Types.Add(viewClass);
                    unit.Namespaces.Add(viewsNamespace);
                    viewClass.Members.Clear();
                    viewClass.Comments.Add(new CodeCommentStatement("当前文件自动生成，禁止修改", true));
                    var uiFields = ViewPrefab.GetComponentsInChildren<ViewField>(true);
                    foreach (var uiField in uiFields)
                    {
                        foreach (var component in uiField.Components)
                        {
                            //添加属性
                            var name = $"{component.name}{component.GetType().GetNiceName()}";
                            CreateProperty(viewClass, component.GetType(), name, UIPathFind.GetPathWithoutCanvasEnvironment(component.transform));
                        }
                        if (uiField.GroupType != GroupType.Default)
                        {
                            var groupRoot = $"{folder}/{ViewPrefab.name}/Groups";
                            if (!Directory.Exists(groupRoot))
                            {
                                Directory.CreateDirectory(groupRoot);
                            }
                            var groupScript = $"{groupRoot}/{uiField.transform.parent.name}_{uiField.transform.name}Group.cs";
                            if (!File.Exists(groupScript))
                            {
                                CodeNamespace groupNamespace = new CodeNamespace();
                                CodeTypeDeclaration group = new CodeTypeDeclaration();
                                group.Attributes = MemberAttributes.Public;
                                group.IsPartial = true;
                                group.BaseTypes.Add(new CodeTypeReference(typeof(BaseGroup)));
                                group.Name = $"{uiField.transform.parent.name}_{uiField.transform.name}{uiField.GroupType.ToString()}Group";
                                groupNamespace.Types.Add(group);
                                groupNamespace.Imports.Add(new CodeNamespaceImport("WingjoyFramework.UIFramework.Runtime.Views"));
                                CodeGeneratorOptions autoScriptOptions = new CodeGeneratorOptions();
                                autoScriptOptions.BracingStyle = "C";
                                autoScriptOptions.BlankLinesBetweenMembers = true;
                                CodeMemberMethod Init = CreateMethod(group, "Init", MemberAttributes.Public | MemberAttributes.Override);
                                switch (uiField.GroupType)
                                {
                                    //滑动区域(动态生成孩子)
                                    case GroupType.ScrollArea:
                                        CreateMethod(group, "SpawnChild");
                                        break;
                                    //按钮交互
                                    case GroupType.ButtonArea:
                                        var Buttons = uiField.transform.GetComponentsInChildren<Button>(true).ToList();
                                        foreach (var btn in Buttons)
                                        {
                                            group.Members.Add(CreateField(typeof(Button), btn.name, MemberAttributes.Private));
                                            CreateMethod(group, $"{btn.transform.name}_BtnClick");
                                            Init.Statements.Add(new CodeExpressionStatement(new CodeSnippetExpression($"m_{btn.name}.onClick.AddListener({btn.transform.name}_BtnClick)")));
                                        }
                                        break;
                                    //子窗口
                                    case GroupType.GeneralWindow:
                                        CreateMethod(group, "LogicAction");
                                        break;
                                    default:
                                        break;
                                }
                                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(groupScript))
                                {
                                    provider.GenerateCodeFromNamespace(groupNamespace, sw, logicScriptOptions);
                                }
                            }
                        }
                    }
                    //构造参数
                    CodeConstructor ctr = new CodeConstructor();
                    CodeParameterDeclarationExpression mTransform = new CodeParameterDeclarationExpression(typeof(Transform), "mTransform");
                    CodeParameterDeclarationExpression mUIViewType = new CodeParameterDeclarationExpression(typeof(UIViewType), "mViewType");
                    ctr.Attributes = MemberAttributes.Public;
                    ctr.Parameters.AddRange(new CodeParameterDeclarationExpression[] { mTransform, mUIViewType });
                    ctr.BaseConstructorArgs.AddRange(new CodeExpressionCollection() { new CodeVariableReferenceExpression("mTransform"), new CodeVariableReferenceExpression("mViewType") });
                    viewClass.Members.Add(ctr);
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fieldScript))
                    {
                        provider.GenerateCodeFromCompileUnit(unit, sw, logicScriptOptions);
                    }
                    // 编译
                    CompilerParameters p = new CompilerParameters();
                    p.OutputAssembly = "UIFramework.Runtime";
                    p.ReferencedAssemblies.Add(@"D:\UnityTest\StudyTest\Library\ScriptAssemblies\UIFramework.Runtime.dll");
                    p.ReferencedAssemblies.Add(@"D:\UnityTest\StudyTest\Library\ScriptAssemblies\Assembly-CSharp.dll");
                    p.ReferencedAssemblies.Add(@"D:\Unity2019.3.2\2019.3.2f1\Editor\Data\Managed\UnityEngine.dll");
                    p.ReferencedAssemblies.Add(@"D:\Unity2019.3.2\2019.3.2f1\Editor\Data\Managed\UnityEngine\UnityEngine.CoreModule.dll");
                    CompilerResults res = provider.CompileAssemblyFromDom(p, unit);
                    if (res.Errors.Count == 0)
                    {
                        Debug.Log("编译成功。");

                    }
                    else
                    {
                        foreach (var item in res.Errors)
                        {
                            Debug.Log($"编译错误:{item.ToString()}");
                        }
                    }
                    AssetDatabase.Refresh();
                    ViewScript = AssetDatabase.LoadAssetAtPath<TextAsset>(fieldScript);
                }
            }

            /// <summary>
            /// View逻辑代码生成
            /// </summary>
            [HorizontalGroup("Meta"), Button(Name = "Install"), ShowIf("@ViewScript!=null")]
            public void Install()
            {
                string folder = "Assets/WingjoyFramework/UIFramework/Runtime/Views";
                string logicScript = $"{folder}/{ViewPrefab.name}/{ViewPrefab.name}Logic.cs";
                DirectoryInfo root = new DirectoryInfo(folder);
                var directoriesInfo = root.GetDirectories();
                CodeCompileUnit unit = new CodeCompileUnit();
                CodeNamespace sampleNamespace = new CodeNamespace("WingjoyFramework.UIFramework.Runtime.Views");
                sampleNamespace.Imports.Add(new CodeNamespaceImport("System"));
                sampleNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                CodeTypeDeclaration logicClass = new CodeTypeDeclaration(ViewPrefab.name);
                sampleNamespace.Types.Add(logicClass);
                unit.Namespaces.Add(sampleNamespace);
                //生成代码
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                logicClass.BaseTypes.Add(new CodeTypeReference(typeof(BaseView)));
                logicClass.TypeAttributes = TypeAttributes.Public;
                logicClass.IsClass = true;
                logicClass.IsPartial = true;
                if (!File.Exists(logicScript))
                {
                    var methodInfos = typeof(BaseView).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    foreach (var methodInfo in methodInfos)
                    {
                        if (methodInfo.IsVirtual)
                        {
                            CodeMemberMethod overrideMethod = new CodeMemberMethod();
                            overrideMethod.Name = methodInfo.Name;
                            overrideMethod.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            overrideMethod.ReturnType = new CodeTypeReference(methodInfo.ReturnType);
                            foreach (var parameterInfo in methodInfo.GetParameters())
                            {
                                overrideMethod.Parameters.Add(new CodeParameterDeclarationExpression(parameterInfo.ParameterType, parameterInfo.Name));
                            }
                            CodeMethodInvokeExpression methodInvokeExpression = new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), methodInfo.Name);
                            overrideMethod.Statements.Add(new CodeExpressionStatement(methodInvokeExpression));
                            if (methodInfo.Name.Contains("Init"))
                            {
                                var uiFields = ViewPrefab.GetComponentsInChildren<ViewField>(true);
                                //字段赋值
                                foreach (var uiField in uiFields)
                                {
                                    foreach (var component in uiField.Components)
                                    {
                                        var name = $"{component.name}{component.GetType().GetNiceName()}";
                                        var path = UIPathFind.GetPath(component.transform).Replace($"/{ViewPrefab.name}/", "");
                                        CodeAssignStatement codeAssign = new CodeAssignStatement();
                                        CodeVariableReferenceExpression field = new CodeVariableReferenceExpression($"m_{name}");
                                        codeAssign.Left = field;
                                        codeAssign.Right = new CodeSnippetExpression($"_transform.Find(\"{path}\").GetComponent<{component.GetType()}>()");
                                        overrideMethod.Statements.Add(codeAssign);
                                    }
                                }
                                CodeSnippetStatement code = new CodeSnippetStatement();
                            }
                            logicClass.Members.Add(overrideMethod);
                        }
                    }

                    CodeGeneratorOptions autoScriptOptions = new CodeGeneratorOptions();
                    autoScriptOptions.BracingStyle = "C";
                    autoScriptOptions.BlankLinesBetweenMembers = true;

                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(logicScript))
                    {
                        provider.GenerateCodeFromCompileUnit(unit, sw, autoScriptOptions);
                    }
                    Debug.Log(AssetDatabase.GetAssetPath(ViewPrefab));
                    //添加自身类型
                }
                ViewConfig.Instance.AddView(ViewPrefab.name, AssetDatabase.GetAssetPath(ViewPrefab));
                //覆盖工厂类
                CodeNamespace factoryNamespace = new CodeNamespace("WingjoyFramework.UIFramework.Runtime.Factory");
                CodeTypeDeclaration factory = new CodeTypeDeclaration("FactoryView");
                factoryNamespace.Types.Add(factory);
                if (ViewPrefab != null)
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (assembly.GetName().Name == "UIFramework.Runtime")
                        {
                            //覆盖工厂类
                            factory.Comments.Add(new CodeCommentStatement("View工厂类---------------当前文件自动生成，禁止修改", true));
                            factory.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                            CodeMemberField baseView = new CodeMemberField(typeof(BaseView), "baseView");
                            baseView.Attributes = MemberAttributes.Public | MemberAttributes.Static;
                            factory.Members.Add(baseView);
                            factoryNamespace.Imports.Add(new CodeNamespaceImport("System"));
                            factoryNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
                            factoryNamespace.Imports.Add(new CodeNamespaceImport("System.Collections"));
                            CodeMemberMethod codeFactoryMemberMethod = new CodeMemberMethod();
                            CodeParameterDeclarationExpression p1 = new CodeParameterDeclarationExpression(typeof(Transform), "transform");
                            CodeParameterDeclarationExpression p2 = new CodeParameterDeclarationExpression(typeof(UIViewType), "uIViewType");
                            codeFactoryMemberMethod.Parameters.AddRange(new CodeParameterDeclarationExpression[] { p1, p2 });
                            codeFactoryMemberMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
                            codeFactoryMemberMethod.ReturnType = new CodeTypeReference(typeof(BaseView));
                            codeFactoryMemberMethod.Name = "CreateView";
                            factory.Members.Add(codeFactoryMemberMethod);
                            foreach (var item in directoriesInfo)
                            {
                                //生成工厂条件语句
                                CodeBinaryOperatorExpression codeBinaryOperator = new CodeBinaryOperatorExpression();
                                codeBinaryOperator.Operator = CodeBinaryOperatorType.IdentityEquality;
                                codeBinaryOperator.Left = new CodeVariableReferenceExpression("uIViewType");
                                //var viewType = Enum.Parse(uiViewType, item.Name);
                                codeBinaryOperator.Right = new CodeSnippetExpression($"UIViewType.{item.Name}");
                                CodeConditionStatement codeCondition = new CodeConditionStatement();
                                codeCondition.Condition = codeBinaryOperator;
                                //找到生成的type
                                var viewLogicType = assembly.GetType($"WingjoyFramework.UIFramework.Runtime.Views.{item.Name}");

                                codeCondition.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("baseView"), new CodeObjectCreateExpression(viewLogicType
                                    , new CodeArgumentReferenceExpression("transform"), new CodeArgumentReferenceExpression("uIViewType"))));
                                codeCondition.TrueStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("baseView")));
                                codeFactoryMemberMethod.Statements.Add(codeCondition);
                            }
                            //工厂返回
                            codeFactoryMemberMethod.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("baseView")));
                        }

                    }
                    CodeGeneratorOptions autoScriptOptions = new CodeGeneratorOptions();
                    autoScriptOptions.BracingStyle = "C";
                    factoryNamespace.Imports.Add(new CodeNamespaceImport("WingjoyFramework.UIFramework.Runtime.Enums"));
                    factoryNamespace.Imports.Add(new CodeNamespaceImport("WingjoyFramework.UIFramework.Runtime.Views"));
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter("Assets/WingjoyFramework/UIFramework/Runtime/Factory/FactoryView.cs"))
                    {
                        provider.GenerateCodeFromNamespace(factoryNamespace, sw, autoScriptOptions);
                    }

                    EditorUtility.SetDirty(ViewPrefab);
                    AssetDatabase.SaveAssets();
                }



                AssetDatabase.Refresh();
            }


            /// <summary>
            /// 创建字段
            /// </summary>
            /// <param name="type">类型</param>
            /// <param name="name">名称</param>
            /// <param name="attribute">作用域</param>
            /// <returns></returns>
            public CodeMemberField CreateField(Type type, string name, MemberAttributes attribute)
            {
                CodeMemberField field = new CodeMemberField(type, $"m_{name}")
                {
                    Attributes = attribute
                };
                //  field.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializeField))));
                return field;
            }
            /// <summary>
            ///  创建字段
            /// </summary>
            /// <param name="component">组件</param>
            /// <param name="attribute">attribute</param>
            /// <returns></returns>
            public CodeMemberField CreateField(Component component, MemberAttributes attribute)
            {
                var name = $"{component.name}{component.GetType().GetNiceName()}";
                CodeMemberField field = new CodeMemberField(component.GetType(), $"m_{name}");
                field.Comments.Add(new CodeCommentStatement(UIPathFind.GetPathWithoutCanvasEnvironment(component.transform), true));
                field.Attributes = attribute;
                field.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializeField))));
                return field;
            }


            /// <summary>
            /// 为指定类创建属性
            /// </summary>
            /// <param name="mClass"></param>
            /// <param name="type"></param>
            /// <param name="mName"></param>
            public void CreateProperty(CodeTypeDeclaration mClass, Type type, string mName, string monoPath = "", bool isGet = true, bool isSet = false)
            {
                CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
                codeMemberProperty.Name = mName;
                if (isGet)
                {
                    codeMemberProperty.HasGet = true;
                    var field = CreateField(type, mName, MemberAttributes.Private);
                    field.Comments.Add(new CodeCommentStatement(monoPath, true));
                    codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression($"m_{mName}")));
                    codeMemberProperty.Type = new CodeTypeReference(type);
                    mClass.Members.Add(field);
                }
                if (isSet)
                {
                    codeMemberProperty.HasSet = true;
                }
                codeMemberProperty.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                mClass.Members.Add(codeMemberProperty);
            }


            /// <summary>
            /// 为指定类创建方法
            /// </summary>
            /// <param name="mClass"></param>
            /// <param name="mName"></param>
            /// <param name="attributes"></param>
            /// <returns></returns>
            public CodeMemberMethod CreateMethod(CodeTypeDeclaration mClass, string mName, MemberAttributes attributes = MemberAttributes.Public | MemberAttributes.Final)
            {
                CodeMemberMethod memberMethod = new CodeMemberMethod();
                memberMethod.Name = mName;
                memberMethod.Attributes = attributes;
                mClass.Members.Add(memberMethod);
                return memberMethod;
            }

            /// <summary>
            /// 在方法中为UIField赋值
            /// </summary>
            public void SetField()
            {

            }
        }
    }
}