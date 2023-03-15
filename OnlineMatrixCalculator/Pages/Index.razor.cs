using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using OnlineMatrixCalculator;
using OnlineMatrixCalculator.Shared;
using MudBlazor;

namespace OnlineMatrixCalculator.Pages
{
    public partial class Index
    {
        double[, ]? inputValues;
        const int additionalColumns = 1;
        int privateColumns = 2;
        int privateRows = 2;

        int columns
        {
            get
            {
                return privateColumns;
            }

            set
            {
                if (!(value < 0))
                {
                    if (value > 26)
                    {
                        privateColumns = 26;
                        rows = 26;
                    }
                    else
                    {
                        privateColumns = value;
                        rows = value;
                    }
                }
            }
        }

        int rows
        {
            get
            {
                return privateRows;
            }

            set
            {
                if (!(value < columns))
                {
                    if (value > 26)
                    {
                        privateRows = 26;
                    }
                    else
                    {
                        privateRows = value;
                    }
                }
            }
        }

        int totalColumns
        {
            get
            {
                return columns + additionalColumns;
            }

            set
            {
                columns = value - additionalColumns;
            }
        }

        List<double> result = new List<double>();
        string errorMsg = "";
        string[] letters = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
        protected override async Task OnInitializedAsync()
        {
            ChangeArray();

        }

        private void ChangeArray()
        {
            inputValues = new double[rows, totalColumns];
        }

        private void ColumnChange(int value)
        {
            columns = value;
            ChangeArray();
        }

        private void RowChange(int value)
        {
            rows = value;
            ChangeArray();
        }

        private void Button_OnClick()
        {
            List<double> inputsList = new List<double>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < totalColumns; j++)
                {
                    inputsList.Add(inputValues[i, j]);
                }
            }
            //errorMsg = System.Text.Json.JsonSerializer.Serialize(inputsList);
            try
            {
                result = Solution(totalColumns, rows, inputsList);
                errorMsg = "";
            }
            catch
            {
                errorMsg = "ERROR";
            }
        }

        private void MatrixValue_OnChanged(double value, int columnIndex, int rowIndex)
        {
            inputValues[columnIndex, rowIndex] = value;
        }

        private static List<double> Solution(int length, int width, List<double> num)
        {
            int cR = 1;
            int sL = 0;
            for (int cS = 0; cS < length - 2; cS++)
            {
                double s = Double.MaxValue;
                for (int gS = 0; gS < width - cR + 1; gS++)
                {
                    if ((num[cS + cS * length + gS * length]) != 0 && Math.Abs((num[cS + cS * length + gS * length])) < s)
                    {
                        s = num[cS + cS * length + gS * length];
                        sL = cS + gS + 1;
                    }
                }

                if (sL != cS + 1)
                {
                    List<double> nC1 = new List<double>();
                    List<double> nC2 = new List<double>();
                    for (int i = 0; i < length; i++)
                        nC1.Add(num[length * cS + i]);
                    for (int i = 0; i < length; i++)
                        nC2.Add(num[length * (sL - 1) + i]);
                    for (int i = 0; i < length; i++)
                        num[length * (sL - 1) + i] = nC1[i];
                    for (int i = 0; i < length; i++)
                        num[length * cS + i] = nC2[i];
                }

                int mL = cR;
                for (int i = 0; i < width - cR; i++)
                {
                    if (num[Convert.ToInt32(length) * mL + cR - 1] != 0)
                    {
                        List<double> nM1 = new List<double>();
                        List<double> nM2 = new List<double>();
                        double mF1 = num[length * cS + cS];
                        double mF2 = num[length * (cS + i + 1) + cS];
                        for (int j = 0; j < length; j++)
                            nM1.Add(num[length * cS + j] * mF2);
                        for (int j = 0; j < length; j++)
                            nM2.Add(num[length * (cS + i + 1) + j] * mF1);
                        for (int j = 0; j < length; j++)
                            num[length * (cS + i + 1) + j] = nM1[j] - nM2[j];
                    }

                    mL++;
                }

                cR++;
            }

            List<double> sol = new List<double>();
            for (int j = 0; j < length - 1; j++)
                sol.Add(0);
            for (int i = 0; i < length - 1; i++)
            {
                sol[Convert.ToInt32(length - i - 2)] = num[Convert.ToInt32((width - i) * length - 1)];
                int j = 0;
                for (j = 0; j < i; j++)
                    sol[Convert.ToInt32(length - i - 2)] = sol[Convert.ToInt32(length - i - 2)] - num[Convert.ToInt32((width - i) * length - 2 - j)] * sol[Convert.ToInt32(length - j - 2)];
                sol[Convert.ToInt32(length - i - 2)] = sol[Convert.ToInt32(length - i - 2)] / num[Convert.ToInt32((width - i) * length - 2 - j)];
            }

            return sol;
        }
    }
}