using System;
using System.CodeDom;
using System.Reflection;

namespace DotNetZen.CodeDom
{
	internal class Utils
	{
		/*private Utils()
		{
		}

		public static void ImplementInterfaceImplicit(CodeTypeDeclaration type, System.Type @interface)
		{
			foreach (CodeTypeMember member in ImplementInterfaceCore(@interface))
			{
				if (member is CodeMemberEvent)
					((CodeMemberEvent)(member)).PrivateImplementationType = new CodeTypeReference(@interface);
				else if (member is CodeMemberMethod)
					((CodeMemberMethod)(member)).PrivateImplementationType = new CodeTypeReference(@interface);
				else if (member is CodeMemberProperty)
					((CodeMemberProperty)(member)).PrivateImplementationType = new CodeTypeReference(@interface);

				type.Members.Add(member);
			}
		}

		public static void ImplementInterfaceExplicit(CodeTypeDeclaration type, System.Type @interface)
		{
			foreach (CodeTypeMember member in ImplementInterfaceCore(@interface))
			{
				if (member is CodeMemberEvent)
					((CodeMemberEvent)(member)).ImplementationTypes.Add(@interface);
				else if (member is CodeMemberMethod)
					((CodeMemberMethod)(member)).ImplementationTypes.Add(@interface);
				else if (member is CodeMemberProperty)
					((CodeMemberProperty)(member)).ImplementationTypes.Add(@interface);

				member.Attributes &= ~MemberAttributes.ScopeMask;
				member.Attributes |= MemberAttributes.Public;

				type.Members.Add(member);
			}
		}

		private static MemberAttributes GetMemberAttributes(MethodAttributes methodAttribs)
		{
			MemberAttributes attribs;
			attribs &= MemberAttributes
		}

		private static CodeMemberMethod CreateCodeMemberMethod(MethodInfo methodInfo)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			method.Attributes = GetMemberAttributes(methodInfo.Attributes);
			method.Name = methodInfo.Name;
			
			foreach (ParameterInfo parameter in methodInfo.GetParameters())
			{
				method.Parameters.Add(CloneCodeParameterDeclarationExpression(parameter));
			}

			method.ReturnType = new CodeTypeReference(methodInfo.ReturnType);

			return method;
		}

		private static CodeMemberEvent CreateCodeMemberEvent(EventInfo eventInfo)
		{
			/*CodeMemberEvent @event = new CodeMemberEvent();
			@event.Attributes = GetMemberAttributes(eventInfo.GetAddMethod(true).Attributes);
			@event.Name = eventInfo.Name;
			@event.Type = new CodeTypeReference(eventInfo.EventHandlerType);

			return @event;*//*
			throw new NotImplementedException();
		}

		private static CodeMemberProperty CreateCodeMemberProperty(PropertyInfo propertyInfo)
		{
			throw new NotImplementedException();
		}

		private static CodeTypeMemberCollection ImplementInterfaceCore(System.Type @interface)
		{
			if (!@interface.IsInterface)
				throw new InvalidOperationException("Type not an interface.");

			CodeTypeMemberCollection collection = new CodeTypeMemberCollection();

			foreach (MethodInfo decl in @interface.GetMethods())
			{
				collection.Add(CreateCodeMemberMethod(decl));
			}

			/*foreach (EventInfo decl in @interface.GetEvents())
			{
				collection.Add(CreateCodeMemberEvent(decl));
			}

			foreach (PropertyInfo decl in @interface.GetProperties())
			{
				collection.Add(CreateCodeMemberProperty(decl));
			}*//*

			return collection;
		}*/
	}
}
