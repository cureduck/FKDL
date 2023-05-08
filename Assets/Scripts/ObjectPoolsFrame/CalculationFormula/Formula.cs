using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CH.Formula
{
    public class QuadraticFormula
    {
        private float a;
        private float x1;
        private float x2;
        private float b;

        public QuadraticFormula(float a, float x1, float x2, float b)
        {
            this.a = a;
            this.x1 = x1;
            this.x2 = x2;
            this.b = b;
        }

        public float GetYValue(float x)
        {
            return a * (x - x1) * (x - x2) + b;
        }
    }
}