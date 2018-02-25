using System;
using System.Collections.Generic;

namespace SearchLocals.Model.VSPropertyModel
{
    public class SelfReferenceDetection
    {
        const int MAX_PROPERTY_NAME_REPETITIONS = 3;

        public static bool DoesItLookLikeSelfReference(string propertyName)
        {
            var segments = propertyName.Split('.');

            HashSet<string> withoutDuplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int counter = 0;
            // detect duplicates that are in sequence
            for (int i=1;i<segments.Length;i++)
            {                
                if (String.Equals( segments[i-1], segments[i], StringComparison.Ordinal))
                    counter++;                
                else
                    counter = 0;

                if (counter >= MAX_PROPERTY_NAME_REPETITIONS - 1)
                    return true;
            }

            return false;    
        }
    }
}
