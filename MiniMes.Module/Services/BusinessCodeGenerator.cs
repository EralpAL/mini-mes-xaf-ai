using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System;
using System.Collections;

namespace MiniMes.Module.Services
{
    // Produces the sequential business codes shown in the Code property of the master data
    // classes: WS-1, WS-2, WS-3 and so on. Every Business Object that owns a Code property
    // calls GenerateCode from AfterConstruction and again from OnSaving when the code is empty.
    public static class BusinessCodeGenerator
    {
        public const string CodePropertyName = "Code";

        private const string CodeSeparator = "-";

        // Two objects created at the same moment must not read the same last number, so the
        // read-and-increment step runs one at a time inside this process. The unique index on
        // the Code column remains the final guard against codes created by another process.
        private static readonly object generationLock = new object();

        public static string GenerateCode(Session session, Type objectType, string prefix)
        {
            if (session == null || objectType == null || string.IsNullOrEmpty(prefix))
            {
                return string.Empty;
            }

            lock (generationLock)
            {
                XPClassInfo classInfo = session.GetClassInfo(objectType);

                if (classInfo == null)
                {
                    return string.Empty;
                }

                XPMemberInfo codeMember = classInfo.FindMember(CodePropertyName);

                if (codeMember == null)
                {
                    return string.Empty;
                }

                string codeStart = prefix + CodeSeparator;
                int nextNumber = GetHighestUsedNumber(session, classInfo, codeMember, codeStart) + 1;

                return codeStart + nextNumber;
            }
        }

        private static int GetHighestUsedNumber(Session session, XPClassInfo classInfo, XPMemberInfo codeMember, string codeStart)
        {
            CriteriaOperator criteria = new FunctionOperator(
                FunctionOperatorType.StartsWith,
                new OperandProperty(CodePropertyName),
                new OperandValue(codeStart));

            // The last argument evaluates the criteria in the current transaction, so objects
            // that were created in this Object Space but are not committed yet are counted too.
            ICollection storedObjects = session.GetObjects(classInfo, criteria, null, 0, false, true);

            if (storedObjects == null || storedObjects.Count == 0)
            {
                return 0;
            }

            object[] storedObjectArray = new object[storedObjects.Count];
            storedObjects.CopyTo(storedObjectArray, 0);

            int highestNumber = 0;

            for (int i = 0; i < storedObjectArray.Length; i++)
            {
                string storedCode = (string)codeMember.GetValue(storedObjectArray[i]);
                int storedNumber = ReadNumberPart(storedCode, codeStart);

                if (storedNumber > highestNumber)
                {
                    highestNumber = storedNumber;
                }
            }

            return highestNumber;
        }

        // Codes that do not follow the "<prefix>-<number>" format belong to older or imported
        // records and must not influence the next number.
        private static int ReadNumberPart(string code, string codeStart)
        {
            if (string.IsNullOrEmpty(code))
            {
                return 0;
            }

            if (!code.StartsWith(codeStart, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            string numberPart = code.Substring(codeStart.Length);
            int number = 0;

            if (!int.TryParse(numberPart, out number))
            {
                return 0;
            }

            if (number < 0)
            {
                return 0;
            }

            return number;
        }
    }
}
