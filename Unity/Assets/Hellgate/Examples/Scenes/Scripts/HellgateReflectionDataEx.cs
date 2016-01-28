//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
//               Hellgate Framework
// Copyright © Uniqtem Co., Ltd.
//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*

using UnityEngine;
using System.Collections;

public class HellgateReflectionDataEx
{
    public class Data
    {
        public class DataArray
        {
            private string name = "";
            private int value = 0;
            private Test test = null;

            public string Name {
                get {
                    return name;
                }
            }

            public int Value {
                get {
                    return value;
                }
            }

            public Test _Test {
                get {
                    return test;
                }
            }
        }

        private string dataFirst = "";
        private string dataSecond = "";
        private string dataThird = "";
        private DataArray[] dataArray = null;

        public string DataFirst {
            get {
                return dataFirst;
            }
        }

        public string DataSecond {
            get {
                return dataSecond;
            }
        }

        public string DataThird {
            get {
                return dataThird;
            }
        }

        public DataArray[] _DataArray {
            get {
                return dataArray;
            }
        }
    }

    public class Test
    {
        private string testFirst = "";
        private string testSecond = "";

        public string TestFirst {
            get {
                return testFirst;
            }
        }

        public string TestSecond {
            get {
                return testSecond;
            }
        }
    }

    private string status = "";
    private string url = "";
    private int intTest = 0;
    private double doubleTest = 0;
    private bool boolTest = false;
    private int[] intArray = null;
    private double[] doubleArray = null;
    private string[] stringArray = null;
    private Data data = null;
    private Test test = null;

    public string Status {
        get {
            return status;
        }
    }

    public string Url {
        get {
            return url;
        }
    }

    public int IntTest {
        get {
            return intTest;
        }
    }

    public double DoubleTest {
        get {
            return doubleTest;
        }
    }

    public bool BoolTest {
        get {
            return boolTest;
        }
    }

    public int[] IntArray {
        get {
            return intArray;
        }
    }

    public double[] DoubleArray {
        get {
            return doubleArray;
        }
    }

    public string[] StringArray {
        get {
            return stringArray;
        }
    }

    public Data _Data {
        get {
            return data;
        }
    }

    public Test _Test {
        get {
            return test;
        }
    }
}
