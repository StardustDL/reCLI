﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace reCLI.Core
{
    public static class StringMatcher
    {
        public static int Score(string source, string target)
        {
            if (!string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(target))
            {
                FuzzyMatcher matcher = FuzzyMatcher.Create(target);
                var score = matcher.Evaluate(source).Score;
                return score;
            }
            else
            {
                return 0;
            }
        }

        public static bool IsMatch(string source, string target)
        {
            return Score(source, target) > 0;
        }
    }
}
