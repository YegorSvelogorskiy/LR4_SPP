﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestsGeneratorLibrary.Structure
{
     public class TreeBuilder
     {
          private readonly string fileText;

          public TreeBuilder(string fileText)
          {
               this.fileText = fileText;
          }

          public TreeStructure GetTreeStructure()
          {
               SyntaxTree programSyntaxTree = CSharpSyntaxTree.ParseText(fileText);
               CompilationUnitSyntax root = programSyntaxTree.GetCompilationUnitRoot();

               var classDeclarations = from classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>()
                                       select classDeclaration;

               return new TreeStructure(GetClasses(classDeclarations));
          }

          private IEnumerable<ClassInfo> GetClasses(IEnumerable<ClassDeclarationSyntax> classDeclarations)
          {
               List<ClassInfo> classes = new List<ClassInfo>();

               foreach (ClassDeclarationSyntax classDeclaration in classDeclarations)
               {
                    var publicMethods = from methodDeclaration in classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>()
                                        where methodDeclaration.Modifiers.Any(x => x.ValueText == "public") == true
                                        select methodDeclaration;

                    var namespaceDeclaration = (NamespaceDeclarationSyntax)classDeclaration.Parent;

                    string namespaceName = namespaceDeclaration.Name.ToString();
                    string className = classDeclaration.Identifier.ValueText;
                    IEnumerable<MethodInfo> methods = GetMethods(publicMethods);

                    classes.Add(new ClassInfo(namespaceName, className, methods));
               }

               return classes;
          }

          private IEnumerable<MethodInfo> GetMethods(IEnumerable<MethodDeclarationSyntax> methodDeclarations)
          {
               List<MethodInfo> methods = new List<MethodInfo>();

               foreach (MethodDeclarationSyntax methodDeclaration in methodDeclarations)
               {
                    string methodName = methodDeclaration.Identifier.ValueText;
                    methods.Add(new MethodInfo(methodName));
               }

               return methods;
          }
     }
}
