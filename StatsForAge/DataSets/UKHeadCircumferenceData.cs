using System;
namespace StatsForAge.DataSets
{
    public sealed class UKHeadCircumferenceData : CentileData
    {
        internal UKHeadCircumferenceData()
        {
            base.AgeMonthsRange = new GenderRange
            {
                MaleRange = new AgeRange(3, 216),
                FemaleRange = new AgeRange(3, 204)
            };
        }
        protected override LMS LMSForGestAge(int gestAgeWeeks, bool isMale)
        {
            if (isMale)
            {
                switch (gestAgeWeeks)
                {
                    case 23:
                        return new LMS { L = 1, M = 21.63267, S = 0.04465746 };
                    case 24:
                        return new LMS { L = 1, M = 22.66466, S = 0.04660628 };
                    case 25:
                        return new LMS { L = 1, M = 23.69877, S = 0.04852065 };
                    case 26:
                        return new LMS { L = 1, M = 24.7339, S = 0.05032903 };
                    case 27:
                        return new LMS { L = 1, M = 25.7583, S = 0.05183461 };
                    case 28:
                        return new LMS { L = 1, M = 26.73601, S = 0.05277836 };
                    case 29:
                        return new LMS { L = 1, M = 27.63749, S = 0.05302389 };
                    case 30:
                        return new LMS { L = 1, M = 28.48013, S = 0.05259097 };
                    case 31:
                        return new LMS { L = 1, M = 29.28385, S = 0.05156045 };
                    case 32:
                        return new LMS { L = 1, M = 30.05091, S = 0.05001475 };
                    case 33:
                        return new LMS { L = 1, M = 30.78144, S = 0.04810781 };
                    case 34:
                        return new LMS { L = 1, M = 31.47371, S = 0.04598968 };
                    case 35:
                        return new LMS { L = 1, M = 32.12924, S = 0.04376877 };
                    case 36:
                        return new LMS { L = 1, M = 32.75117, S = 0.04152338 };
                    case 37:
                        return new LMS { L = 1, M = 33.34215, S = 0.03930918 };
                    case 38:
                        return new LMS { L = 1, M = 33.89909, S = 0.03717904 };
                    case 39:
                        return new LMS { L = 1, M = 34.41823, S = 0.03517313 };
                    case 40:
                        return new LMS { L = 1, M = 34.91071, S = 0.0332788 };
                    case 41:
                        return new LMS { L = 1, M = 35.38669, S = 0.0314591 };
                    case 42:
                        return new LMS { L = 1, M = 35.85209, S = 0.02967462 };
                    case 43:
                        return new LMS { L = 1, M = 36.5216, S = 0.03197 };
                    default:
                        throw new ArgumentOutOfRangeException("gestAgeWeeks");
                }
            }
            switch (gestAgeWeeks)//Female
            {
                case 23:
                    return new LMS { L = 1, M = 21.10156, S = 0.0662953 };
                case 24:
                    return new LMS { L = 1, M = 22.16809, S = 0.06482162 };
                case 25:
                    return new LMS { L = 1, M = 23.23329, S = 0.06331395 };
                case 26:
                    return new LMS { L = 1, M = 24.29014, S = 0.06173639 };
                case 27:
                    return new LMS { L = 1, M = 25.32564, S = 0.06007728 };
                case 28:
                    return new LMS { L = 1, M = 26.31602, S = 0.05835389 };
                case 29:
                    return new LMS { L = 1, M = 27.25686, S = 0.05650073 };
                case 30:
                    return new LMS { L = 1, M = 28.15286, S = 0.05443045 };
                case 31:
                    return new LMS { L = 1, M = 29.00924, S = 0.05212006 };
                case 32:
                    return new LMS { L = 1, M = 29.81712, S = 0.04956586 };
                case 33:
                    return new LMS { L = 1, M = 30.56807, S = 0.04687613 };
                case 34:
                    return new LMS { L = 1, M = 31.25904, S = 0.04415755 };
                case 35:
                    return new LMS { L = 1, M = 31.88534, S = 0.04150212 };
                case 36:
                    return new LMS { L = 1, M = 32.44534, S = 0.03897662 };
                case 37:
                    return new LMS { L = 1, M = 32.94206, S = 0.03661936 };
                case 38:
                    return new LMS { L = 1, M = 33.38124, S = 0.0344432 };
                case 39:
                    return new LMS { L = 1, M = 33.77464, S = 0.03242938 };
                case 40:
                    return new LMS { L = 1, M = 34.13042, S = 0.03055761 };
                case 41:
                    return new LMS { L = 1, M = 34.4652, S = 0.02876645 };
                case 42:
                    return new LMS { L = 1, M = 34.79057, S = 0.02701415 };
                case 43:
                    return new LMS { L = 1, M = 35.843, S = 0.03231 };
                default:
                    throw new ArgumentException("gestAgeWeeks");
            }
        }

        protected override LMS LMSForAgeWeeks(int ageWeeks, bool isMale)
        {
            if (isMale)
            {
                switch (ageWeeks)
                {
                    case 4:
                        return new LMS { L = 1, M = 37.0926, S = 0.03148 };
                    case 5:
                        return new LMS { L = 1, M = 37.601, S = 0.03107 };
                    case 6:
                        return new LMS { L = 1, M = 38.0609, S = 0.03072 };
                    case 7:
                        return new LMS { L = 1, M = 38.4824, S = 0.03041 };
                    case 8:
                        return new LMS { L = 1, M = 38.8724, S = 0.03014 };
                    case 9:
                        return new LMS { L = 1, M = 39.2368, S = 0.0299 };
                    case 10:
                        return new LMS { L = 1, M = 39.5797, S = 0.02969 };
                    case 11:
                        return new LMS { L = 1, M = 39.9033, S = 0.0295 };
                    case 12:
                        return new LMS { L = 1, M = 40.2096, S = 0.02933 };
                    case 13:
                        return new LMS { L = 1, M = 40.5008, S = 0.02918 };
                    default:
                        throw new ArgumentOutOfRangeException("ageWeeks");
                }
            }
            switch (ageWeeks) //Female
            {
                case 4:
                    return new LMS { L = 1, M = 36.3761, S = 0.03215 };
                case 5:
                    return new LMS { L = 1, M = 36.8472, S = 0.03202 };
                case 6:
                    return new LMS { L = 1, M = 37.2711, S = 0.03191 };
                case 7:
                    return new LMS { L = 1, M = 37.6584, S = 0.03182 };
                case 8:
                    return new LMS { L = 1, M = 38.0167, S = 0.03173 };
                case 9:
                    return new LMS { L = 1, M = 38.3516, S = 0.03166 };
                case 10:
                    return new LMS { L = 1, M = 38.6673, S = 0.03158 };
                case 11:
                    return new LMS { L = 1, M = 38.9661, S = 0.03152 };
                case 12:
                    return new LMS { L = 1, M = 39.2501, S = 0.03146 };
                case 13:
                    return new LMS { L = 1, M = 39.521, S = 0.0314 };
                default:
                    throw new ArgumentOutOfRangeException("ageWeeks");
            }
        }

        protected override LMS LMSForAgeMonths(int ageMonths, bool isMale)
        {
            if (isMale)
            {
                switch (ageMonths)
                {
                    case 3:
                        return new LMS { L = 1, M = 40.5135, S = 0.02918 };
                    case 4:
                        return new LMS { L = 1, M = 41.6317, S = 0.02868 };
                    case 5:
                        return new LMS { L = 1, M = 42.5576, S = 0.02837 };
                    case 6:
                        return new LMS { L = 1, M = 43.3306, S = 0.02817 };
                    case 7:
                        return new LMS { L = 1, M = 43.9803, S = 0.02804 };
                    case 8:
                        return new LMS { L = 1, M = 44.53, S = 0.02796 };
                    case 9:
                        return new LMS { L = 1, M = 44.9998, S = 0.02792 };
                    case 10:
                        return new LMS { L = 1, M = 45.4051, S = 0.0279 };
                    case 11:
                        return new LMS { L = 1, M = 45.7573, S = 0.02789 };
                    case 12:
                        return new LMS { L = 1, M = 46.0661, S = 0.02789 };
                    case 13:
                        return new LMS { L = 1, M = 46.3395, S = 0.02789 };
                    case 14:
                        return new LMS { L = 1, M = 46.5844, S = 0.02791 };
                    case 15:
                        return new LMS { L = 1, M = 46.806, S = 0.02792 };
                    case 16:
                        return new LMS { L = 1, M = 47.0088, S = 0.02795 };
                    case 17:
                        return new LMS { L = 1, M = 47.1962, S = 0.02797 };
                    case 18:
                        return new LMS { L = 1, M = 47.3711, S = 0.028 };
                    case 19:
                        return new LMS { L = 1, M = 47.5357, S = 0.02803 };
                    case 20:
                        return new LMS { L = 1, M = 47.6919, S = 0.02806 };
                    case 21:
                        return new LMS { L = 1, M = 47.8408, S = 0.0281 };
                    case 22:
                        return new LMS { L = 1, M = 47.9833, S = 0.02813 };
                    case 23:
                        return new LMS { L = 1, M = 48.1201, S = 0.02817 };
                    case 24:
                        return new LMS { L = 1, M = 48.2515, S = 0.02821 };
                    case 25:
                        return new LMS { L = 1, M = 48.3777, S = 0.02825 };
                    case 26:
                        return new LMS { L = 1, M = 48.4989, S = 0.0283 };
                    case 27:
                        return new LMS { L = 1, M = 48.6151, S = 0.02834 };
                    case 28:
                        return new LMS { L = 1, M = 48.7264, S = 0.02838 };
                    case 29:
                        return new LMS { L = 1, M = 48.8331, S = 0.02842 };
                    case 30:
                        return new LMS { L = 1, M = 48.9351, S = 0.02847 };
                    case 31:
                        return new LMS { L = 1, M = 49.0327, S = 0.02851 };
                    case 32:
                        return new LMS { L = 1, M = 49.126, S = 0.02855 };
                    case 33:
                        return new LMS { L = 1, M = 49.2153, S = 0.02859 };
                    case 34:
                        return new LMS { L = 1, M = 49.3007, S = 0.02863 };
                    case 35:
                        return new LMS { L = 1, M = 49.3826, S = 0.02867 };
                    case 36:
                        return new LMS { L = 1, M = 49.4612, S = 0.02871 };
                    case 37:
                        return new LMS { L = 1, M = 49.5367, S = 0.02875 };
                    case 38:
                        return new LMS { L = 1, M = 49.6093, S = 0.02878 };
                    case 39:
                        return new LMS { L = 1, M = 49.6791, S = 0.02882 };
                    case 40:
                        return new LMS { L = 1, M = 49.7465, S = 0.02886 };
                    case 41:
                        return new LMS { L = 1, M = 49.8116, S = 0.02889 };
                    case 42:
                        return new LMS { L = 1, M = 49.8745, S = 0.02893 };
                    case 43:
                        return new LMS { L = 1, M = 49.9354, S = 0.02896 };
                    case 44:
                        return new LMS { L = 1, M = 49.9942, S = 0.02899 };
                    case 45:
                        return new LMS { L = 1, M = 50.0512, S = 0.02903 };
                    case 46:
                        return new LMS { L = 1, M = 50.1064, S = 0.02906 };
                    case 47:
                        return new LMS { L = 1, M = 50.1598, S = 0.02909 };
                    case 48:
                        return new LMS { L = 1, M = 50.2115, S = 0.02912 };
                    case 49:
                        return new LMS { L = 1, M = 52.215, S = 0.02809 };
                    case 50:
                        return new LMS { L = 1, M = 52.315, S = 0.02818 };
                    case 51:
                        return new LMS { L = 1, M = 52.362, S = 0.02822 };
                    case 52:
                        return new LMS { L = 1, M = 52.409, S = 0.02827 };
                    case 53:
                        return new LMS { L = 1, M = 52.455, S = 0.02831 };
                    case 54:
                        return new LMS { L = 1, M = 52.499, S = 0.02835 };
                    case 55:
                        return new LMS { L = 1, M = 52.543, S = 0.02839 };
                    case 56:
                        return new LMS { L = 1, M = 52.585, S = 0.02842 };
                    case 57:
                        return new LMS { L = 1, M = 52.626, S = 0.02846 };
                    case 58:
                        return new LMS { L = 1, M = 52.667, S = 0.02849 };
                    case 59:
                        return new LMS { L = 1, M = 52.708, S = 0.02852 };
                    case 60:
                        return new LMS { L = 1, M = 52.747, S = 0.02855 };
                    case 61:
                        return new LMS { L = 1, M = 52.786, S = 0.02858 };
                    case 62:
                        return new LMS { L = 1, M = 52.823, S = 0.02861 };
                    case 63:
                        return new LMS { L = 1, M = 52.86, S = 0.02864 };
                    case 64:
                        return new LMS { L = 1, M = 52.896, S = 0.02867 };
                    case 65:
                        return new LMS { L = 1, M = 52.932, S = 0.02869 };
                    case 66:
                        return new LMS { L = 1, M = 52.967, S = 0.02872 };
                    case 67:
                        return new LMS { L = 1, M = 53.002, S = 0.02874 };
                    case 68:
                        return new LMS { L = 1, M = 53.036, S = 0.02876 };
                    case 69:
                        return new LMS { L = 1, M = 53.07, S = 0.02878 };
                    case 70:
                        return new LMS { L = 1, M = 53.103, S = 0.0288 };
                    case 71:
                        return new LMS { L = 1, M = 53.136, S = 0.02883 };
                    case 72:
                        return new LMS { L = 1, M = 53.168, S = 0.02884 };
                    case 73:
                        return new LMS { L = 1, M = 53.2, S = 0.02886 };
                    case 74:
                        return new LMS { L = 1, M = 53.232, S = 0.02888 };
                    case 75:
                        return new LMS { L = 1, M = 53.264, S = 0.0289 };
                    case 76:
                        return new LMS { L = 1, M = 53.294, S = 0.02892 };
                    case 77:
                        return new LMS { L = 1, M = 53.325, S = 0.02893 };
                    case 78:
                        return new LMS { L = 1, M = 53.356, S = 0.02895 };
                    case 79:
                        return new LMS { L = 1, M = 53.387, S = 0.02896 };
                    case 80:
                        return new LMS { L = 1, M = 53.416, S = 0.02898 };
                    case 81:
                        return new LMS { L = 1, M = 53.446, S = 0.02899 };
                    case 82:
                        return new LMS { L = 1, M = 53.476, S = 0.029 };
                    case 83:
                        return new LMS { L = 1, M = 53.505, S = 0.02902 };
                    case 84:
                        return new LMS { L = 1, M = 53.534, S = 0.02903 };
                    case 85:
                        return new LMS { L = 1, M = 53.563, S = 0.02904 };
                    case 86:
                        return new LMS { L = 1, M = 53.591, S = 0.02905 };
                    case 87:
                        return new LMS { L = 1, M = 53.62, S = 0.02907 };
                    case 88:
                        return new LMS { L = 1, M = 53.649, S = 0.02908 };
                    case 89:
                        return new LMS { L = 1, M = 53.676, S = 0.02909 };
                    case 90:
                        return new LMS { L = 1, M = 53.704, S = 0.0291 };
                    case 91:
                        return new LMS { L = 1, M = 53.732, S = 0.02911 };
                    case 92:
                        return new LMS { L = 1, M = 53.76, S = 0.02912 };
                    case 93:
                        return new LMS { L = 1, M = 53.787, S = 0.02913 };
                    case 94:
                        return new LMS { L = 1, M = 53.814, S = 0.02914 };
                    case 95:
                        return new LMS { L = 1, M = 53.841, S = 0.02915 };
                    case 96:
                        return new LMS { L = 1, M = 53.867, S = 0.02916 };
                    case 97:
                        return new LMS { L = 1, M = 53.893, S = 0.02917 };
                    case 98:
                        return new LMS { L = 1, M = 53.92, S = 0.02918 };
                    case 99:
                        return new LMS { L = 1, M = 53.946, S = 0.02919 };
                    case 100:
                        return new LMS { L = 1, M = 53.972, S = 0.0292 };
                    case 101:
                        return new LMS { L = 1, M = 53.998, S = 0.02921 };
                    case 102:
                        return new LMS { L = 1, M = 54.024, S = 0.02922 };
                    case 103:
                        return new LMS { L = 1, M = 54.05, S = 0.02923 };
                    case 104:
                        return new LMS { L = 1, M = 54.076, S = 0.02924 };
                    case 105:
                        return new LMS { L = 1, M = 54.101, S = 0.02925 };
                    case 106:
                        return new LMS { L = 1, M = 54.126, S = 0.02926 };
                    case 107:
                        return new LMS { L = 1, M = 54.152, S = 0.02926 };
                    case 108:
                        return new LMS { L = 1, M = 54.178, S = 0.02927 };
                    case 109:
                        return new LMS { L = 1, M = 54.203, S = 0.02928 };
                    case 110:
                        return new LMS { L = 1, M = 54.229, S = 0.02929 };
                    case 111:
                        return new LMS { L = 1, M = 54.254, S = 0.0293 };
                    case 112:
                        return new LMS { L = 1, M = 54.279, S = 0.02931 };
                    case 113:
                        return new LMS { L = 1, M = 54.305, S = 0.02932 };
                    case 114:
                        return new LMS { L = 1, M = 54.33, S = 0.02933 };
                    case 115:
                        return new LMS { L = 1, M = 54.355, S = 0.02933 };
                    case 116:
                        return new LMS { L = 1, M = 54.38, S = 0.02934 };
                    case 117:
                        return new LMS { L = 1, M = 54.406, S = 0.02935 };
                    case 118:
                        return new LMS { L = 1, M = 54.431, S = 0.02936 };
                    case 119:
                        return new LMS { L = 1, M = 54.457, S = 0.02937 };
                    case 120:
                        return new LMS { L = 1, M = 54.482, S = 0.02938 };
                    case 121:
                        return new LMS { L = 1, M = 54.507, S = 0.02938 };
                    case 122:
                        return new LMS { L = 1, M = 54.533, S = 0.02939 };
                    case 123:
                        return new LMS { L = 1, M = 54.558, S = 0.0294 };
                    case 124:
                        return new LMS { L = 1, M = 54.584, S = 0.02941 };
                    case 125:
                        return new LMS { L = 1, M = 54.61, S = 0.02942 };
                    case 126:
                        return new LMS { L = 1, M = 54.635, S = 0.02943 };
                    case 127:
                        return new LMS { L = 1, M = 54.661, S = 0.02943 };
                    case 128:
                        return new LMS { L = 1, M = 54.687, S = 0.02944 };
                    case 129:
                        return new LMS { L = 1, M = 54.713, S = 0.02945 };
                    case 130:
                        return new LMS { L = 1, M = 54.74, S = 0.02946 };
                    case 131:
                        return new LMS { L = 1, M = 54.766, S = 0.02946 };
                    case 132:
                        return new LMS { L = 1, M = 54.793, S = 0.02947 };
                    case 133:
                        return new LMS { L = 1, M = 54.82, S = 0.02948 };
                    case 134:
                        return new LMS { L = 1, M = 54.846, S = 0.02949 };
                    case 135:
                        return new LMS { L = 1, M = 54.874, S = 0.02949 };
                    case 136:
                        return new LMS { L = 1, M = 54.902, S = 0.0295 };
                    case 137:
                        return new LMS { L = 1, M = 54.929, S = 0.02951 };
                    case 138:
                        return new LMS { L = 1, M = 54.956, S = 0.02952 };
                    case 139:
                        return new LMS { L = 1, M = 54.984, S = 0.02952 };
                    case 140:
                        return new LMS { L = 1, M = 55.012, S = 0.02953 };
                    case 141:
                        return new LMS { L = 1, M = 55.04, S = 0.02954 };
                    case 142:
                        return new LMS { L = 1, M = 55.069, S = 0.02955 };
                    case 143:
                        return new LMS { L = 1, M = 55.098, S = 0.02955 };
                    case 144:
                        return new LMS { L = 1, M = 55.126, S = 0.02956 };
                    case 145:
                        return new LMS { L = 1, M = 55.155, S = 0.02957 };
                    case 146:
                        return new LMS { L = 1, M = 55.184, S = 0.02957 };
                    case 147:
                        return new LMS { L = 1, M = 55.214, S = 0.02958 };
                    case 148:
                        return new LMS { L = 1, M = 55.243, S = 0.02959 };
                    case 149:
                        return new LMS { L = 1, M = 55.272, S = 0.02959 };
                    case 150:
                        return new LMS { L = 1, M = 55.302, S = 0.0296 };
                    case 151:
                        return new LMS { L = 1, M = 55.332, S = 0.02961 };
                    case 152:
                        return new LMS { L = 1, M = 55.362, S = 0.02961 };
                    case 153:
                        return new LMS { L = 1, M = 55.392, S = 0.02962 };
                    case 154:
                        return new LMS { L = 1, M = 55.422, S = 0.02963 };
                    case 155:
                        return new LMS { L = 1, M = 55.453, S = 0.02963 };
                    case 156:
                        return new LMS { L = 1, M = 55.484, S = 0.02964 };
                    case 157:
                        return new LMS { L = 1, M = 55.514, S = 0.02964 };
                    case 158:
                        return new LMS { L = 1, M = 55.546, S = 0.02965 };
                    case 159:
                        return new LMS { L = 1, M = 55.576, S = 0.02966 };
                    case 160:
                        return new LMS { L = 1, M = 55.608, S = 0.02966 };
                    case 161:
                        return new LMS { L = 1, M = 55.639, S = 0.02967 };
                    case 162:
                        return new LMS { L = 1, M = 55.67, S = 0.02967 };
                    case 163:
                        return new LMS { L = 1, M = 55.701, S = 0.02968 };
                    case 164:
                        return new LMS { L = 1, M = 55.732, S = 0.02969 };
                    case 165:
                        return new LMS { L = 1, M = 55.764, S = 0.02969 };
                    case 166:
                        return new LMS { L = 1, M = 55.795, S = 0.0297 };
                    case 167:
                        return new LMS { L = 1, M = 55.827, S = 0.0297 };
                    case 168:
                        return new LMS { L = 1, M = 55.858, S = 0.02971 };
                    case 169:
                        return new LMS { L = 1, M = 55.889, S = 0.02971 };
                    case 170:
                        return new LMS { L = 1, M = 55.921, S = 0.02972 };
                    case 171:
                        return new LMS { L = 1, M = 55.953, S = 0.02972 };
                    case 172:
                        return new LMS { L = 1, M = 55.983, S = 0.02973 };
                    case 173:
                        return new LMS { L = 1, M = 56.014, S = 0.02973 };
                    case 174:
                        return new LMS { L = 1, M = 56.046, S = 0.02974 };
                    case 175:
                        return new LMS { L = 1, M = 56.077, S = 0.02974 };
                    case 176:
                        return new LMS { L = 1, M = 56.108, S = 0.02975 };
                    case 177:
                        return new LMS { L = 1, M = 56.139, S = 0.02975 };
                    case 178:
                        return new LMS { L = 1, M = 56.17, S = 0.02976 };
                    case 179:
                        return new LMS { L = 1, M = 56.201, S = 0.02976 };
                    case 180:
                        return new LMS { L = 1, M = 56.232, S = 0.02977 };
                    case 181:
                        return new LMS { L = 1, M = 56.262, S = 0.02977 };
                    case 182:
                        return new LMS { L = 1, M = 56.293, S = 0.02978 };
                    case 183:
                        return new LMS { L = 1, M = 56.324, S = 0.02978 };
                    case 184:
                        return new LMS { L = 1, M = 56.354, S = 0.02979 };
                    case 185:
                        return new LMS { L = 1, M = 56.384, S = 0.02979 };
                    case 186:
                        return new LMS { L = 1, M = 56.414, S = 0.0298 };
                    case 187:
                        return new LMS { L = 1, M = 56.444, S = 0.0298 };
                    case 188:
                        return new LMS { L = 1, M = 56.474, S = 0.0298 };
                    case 189:
                        return new LMS { L = 1, M = 56.503, S = 0.02981 };
                    case 190:
                        return new LMS { L = 1, M = 56.533, S = 0.02981 };
                    case 191:
                        return new LMS { L = 1, M = 56.562, S = 0.02982 };
                    case 192:
                        return new LMS { L = 1, M = 56.591, S = 0.02982 };
                    case 193:
                        return new LMS { L = 1, M = 56.621, S = 0.02983 };
                    case 194:
                        return new LMS { L = 1, M = 56.65, S = 0.02983 };
                    case 195:
                        return new LMS { L = 1, M = 56.679, S = 0.02984 };
                    case 196:
                        return new LMS { L = 1, M = 56.707, S = 0.02984 };
                    case 197:
                        return new LMS { L = 1, M = 56.736, S = 0.02984 };
                    case 198:
                        return new LMS { L = 1, M = 56.764, S = 0.02985 };
                    case 199:
                        return new LMS { L = 1, M = 56.792, S = 0.02985 };
                    case 200:
                        return new LMS { L = 1, M = 56.821, S = 0.02985 };
                    case 201:
                        return new LMS { L = 1, M = 56.849, S = 0.02986 };
                    case 202:
                        return new LMS { L = 1, M = 56.876, S = 0.02986 };
                    case 203:
                        return new LMS { L = 1, M = 56.904, S = 0.02987 };
                    case 204:
                        return new LMS { L = 1, M = 56.932, S = 0.02987 };
                    case 205:
                        return new LMS { L = 1, M = 56.96, S = 0.02987 };
                    case 206:
                        return new LMS { L = 1, M = 56.987, S = 0.02988 };
                    case 207:
                        return new LMS { L = 1, M = 57.014, S = 0.02988 };
                    case 208:
                        return new LMS { L = 1, M = 57.041, S = 0.02989 };
                    case 209:
                        return new LMS { L = 1, M = 57.068, S = 0.02989 };
                    case 210:
                        return new LMS { L = 1, M = 57.095, S = 0.02989 };
                    case 211:
                        return new LMS { L = 1, M = 57.121, S = 0.0299 };
                    case 212:
                        return new LMS { L = 1, M = 57.148, S = 0.0299 };
                    case 213:
                        return new LMS { L = 1, M = 57.174, S = 0.0299 };
                    case 214:
                        return new LMS { L = 1, M = 57.201, S = 0.02991 };
                    case 215:
                        return new LMS { L = 1, M = 57.227, S = 0.02991 };
                    case 216:
                        return new LMS { L = 1, M = 57.26, S = 0.02991 };
                    default:
                        throw new ArgumentOutOfRangeException("ageMonths");
                }
            }
            switch (ageMonths)
            {
                case 3:
                    return new LMS { L = 1, M = 39.5328, S = 0.0314 };
                case 4:
                    return new LMS { L = 1, M = 40.5817, S = 0.03119 };
                case 5:
                    return new LMS { L = 1, M = 41.459, S = 0.03102 };
                case 6:
                    return new LMS { L = 1, M = 42.1995, S = 0.03087 };
                case 7:
                    return new LMS { L = 1, M = 42.829, S = 0.03075 };
                case 8:
                    return new LMS { L = 1, M = 43.3671, S = 0.03063 };
                case 9:
                    return new LMS { L = 1, M = 43.83, S = 0.03053 };
                case 10:
                    return new LMS { L = 1, M = 44.2319, S = 0.03044 };
                case 11:
                    return new LMS { L = 1, M = 44.5844, S = 0.03035 };
                case 12:
                    return new LMS { L = 1, M = 44.8965, S = 0.03027 };
                case 13:
                    return new LMS { L = 1, M = 45.1752, S = 0.03019 };
                case 14:
                    return new LMS { L = 1, M = 45.4265, S = 0.03012 };
                case 15:
                    return new LMS { L = 1, M = 45.6551, S = 0.03006 };
                case 16:
                    return new LMS { L = 1, M = 45.865, S = 0.02999 };
                case 17:
                    return new LMS { L = 1, M = 46.0598, S = 0.02993 };
                case 18:
                    return new LMS { L = 1, M = 46.2424, S = 0.02987 };
                case 19:
                    return new LMS { L = 1, M = 46.4152, S = 0.02982 };
                case 20:
                    return new LMS { L = 1, M = 46.5801, S = 0.02977 };
                case 21:
                    return new LMS { L = 1, M = 46.7384, S = 0.02972 };
                case 22:
                    return new LMS { L = 1, M = 46.8913, S = 0.02967 };
                case 23:
                    return new LMS { L = 1, M = 47.0391, S = 0.02962 };
                case 24:
                    return new LMS { L = 1, M = 47.1822, S = 0.02957 };
                case 25:
                    return new LMS { L = 1, M = 47.3204, S = 0.02953 };
                case 26:
                    return new LMS { L = 1, M = 47.4536, S = 0.02949 };
                case 27:
                    return new LMS { L = 1, M = 47.5817, S = 0.02945 };
                case 28:
                    return new LMS { L = 1, M = 47.7045, S = 0.02941 };
                case 29:
                    return new LMS { L = 1, M = 47.8219, S = 0.02937 };
                case 30:
                    return new LMS { L = 1, M = 47.934, S = 0.02933 };
                case 31:
                    return new LMS { L = 1, M = 48.041, S = 0.02929 };
                case 32:
                    return new LMS { L = 1, M = 48.1432, S = 0.02926 };
                case 33:
                    return new LMS { L = 1, M = 48.2408, S = 0.02922 };
                case 34:
                    return new LMS { L = 1, M = 48.3343, S = 0.02919 };
                case 35:
                    return new LMS { L = 1, M = 48.4239, S = 0.02915 };
                case 36:
                    return new LMS { L = 1, M = 48.5099, S = 0.02912 };
                case 37:
                    return new LMS { L = 1, M = 48.5926, S = 0.02909 };
                case 38:
                    return new LMS { L = 1, M = 48.6722, S = 0.02906 };
                case 39:
                    return new LMS { L = 1, M = 48.7489, S = 0.02903 };
                case 40:
                    return new LMS { L = 1, M = 48.8228, S = 0.029 };
                case 41:
                    return new LMS { L = 1, M = 48.8941, S = 0.02897 };
                case 42:
                    return new LMS { L = 1, M = 48.9629, S = 0.02894 };
                case 43:
                    return new LMS { L = 1, M = 49.0294, S = 0.02891 };
                case 44:
                    return new LMS { L = 1, M = 49.0937, S = 0.02888 };
                case 45:
                    return new LMS { L = 1, M = 49.156, S = 0.02886 };
                case 46:
                    return new LMS { L = 1, M = 49.2164, S = 0.02883 };
                case 47:
                    return new LMS { L = 1, M = 49.2751, S = 0.0288 };
                case 48:
                    return new LMS { L = 1, M = 49.3321, S = 0.02878 };
                case 49:
                    return new LMS { L = 1, M = 51.104, S = 0.02342 };
                case 50:
                    return new LMS { L = 1, M = 51.208, S = 0.02336 };
                case 51:
                    return new LMS { L = 1, M = 51.258, S = 0.02332 };
                case 52:
                    return new LMS { L = 1, M = 51.309, S = 0.02329 };
                case 53:
                    return new LMS { L = 1, M = 51.358, S = 0.02326 };
                case 54:
                    return new LMS { L = 1, M = 51.406, S = 0.02323 };
                case 55:
                    return new LMS { L = 1, M = 51.454, S = 0.02321 };
                case 56:
                    return new LMS { L = 1, M = 51.501, S = 0.02318 };
                case 57:
                    return new LMS { L = 1, M = 51.546, S = 0.02316 };
                case 58:
                    return new LMS { L = 1, M = 51.591, S = 0.02314 };
                case 59:
                    return new LMS { L = 1, M = 51.637, S = 0.02311 };
                case 60:
                    return new LMS { L = 1, M = 51.681, S = 0.02309 };
                case 61:
                    return new LMS { L = 1, M = 51.725, S = 0.02307 };
                case 62:
                    return new LMS { L = 1, M = 51.768, S = 0.02306 };
                case 63:
                    return new LMS { L = 1, M = 51.81, S = 0.02304 };
                case 64:
                    return new LMS { L = 1, M = 51.852, S = 0.02303 };
                case 65:
                    return new LMS { L = 1, M = 51.893, S = 0.02301 };
                case 66:
                    return new LMS { L = 1, M = 51.934, S = 0.023 };
                case 67:
                    return new LMS { L = 1, M = 51.975, S = 0.02299 };
                case 68:
                    return new LMS { L = 1, M = 52.014, S = 0.02298 };
                case 69:
                    return new LMS { L = 1, M = 52.053, S = 0.02297 };
                case 70:
                    return new LMS { L = 1, M = 52.092, S = 0.02296 };
                case 71:
                    return new LMS { L = 1, M = 52.131, S = 0.02295 };
                case 72:
                    return new LMS { L = 1, M = 52.169, S = 0.02294 };
                case 73:
                    return new LMS { L = 1, M = 52.207, S = 0.02294 };
                case 74:
                    return new LMS { L = 1, M = 52.244, S = 0.02293 };
                case 75:
                    return new LMS { L = 1, M = 52.281, S = 0.02293 };
                case 76:
                    return new LMS { L = 1, M = 52.318, S = 0.02293 };
                case 77:
                    return new LMS { L = 1, M = 52.355, S = 0.02293 };
                case 78:
                    return new LMS { L = 1, M = 52.391, S = 0.02292 };
                case 79:
                    return new LMS { L = 1, M = 52.427, S = 0.02292 };
                case 80:
                    return new LMS { L = 1, M = 52.462, S = 0.02292 };
                case 81:
                    return new LMS { L = 1, M = 52.498, S = 0.02293 };
                case 82:
                    return new LMS { L = 1, M = 52.533, S = 0.02293 };
                case 83:
                    return new LMS { L = 1, M = 52.568, S = 0.02293 };
                case 84:
                    return new LMS { L = 1, M = 52.604, S = 0.02293 };
                case 85:
                    return new LMS { L = 1, M = 52.638, S = 0.02294 };
                case 86:
                    return new LMS { L = 1, M = 52.673, S = 0.02294 };
                case 87:
                    return new LMS { L = 1, M = 52.707, S = 0.02295 };
                case 88:
                    return new LMS { L = 1, M = 52.741, S = 0.02296 };
                case 89:
                    return new LMS { L = 1, M = 52.775, S = 0.02296 };
                case 90:
                    return new LMS { L = 1, M = 52.809, S = 0.02297 };
                case 91:
                    return new LMS { L = 1, M = 52.842, S = 0.02298 };
                case 92:
                    return new LMS { L = 1, M = 52.876, S = 0.02299 };
                case 93:
                    return new LMS { L = 1, M = 52.909, S = 0.023 };
                case 94:
                    return new LMS { L = 1, M = 52.942, S = 0.02301 };
                case 95:
                    return new LMS { L = 1, M = 52.975, S = 0.02302 };
                case 96:
                    return new LMS { L = 1, M = 53.008, S = 0.02303 };
                case 97:
                    return new LMS { L = 1, M = 53.041, S = 0.02304 };
                case 98:
                    return new LMS { L = 1, M = 53.073, S = 0.02305 };
                case 99:
                    return new LMS { L = 1, M = 53.106, S = 0.02306 };
                case 100:
                    return new LMS { L = 1, M = 53.138, S = 0.02308 };
                case 101:
                    return new LMS { L = 1, M = 53.169, S = 0.02309 };
                case 102:
                    return new LMS { L = 1, M = 53.201, S = 0.0231 };
                case 103:
                    return new LMS { L = 1, M = 53.232, S = 0.02312 };
                case 104:
                    return new LMS { L = 1, M = 53.264, S = 0.02313 };
                case 105:
                    return new LMS { L = 1, M = 53.295, S = 0.02315 };
                case 106:
                    return new LMS { L = 1, M = 53.326, S = 0.02316 };
                case 107:
                    return new LMS { L = 1, M = 53.357, S = 0.02318 };
                case 108:
                    return new LMS { L = 1, M = 53.387, S = 0.0232 };
                case 109:
                    return new LMS { L = 1, M = 53.417, S = 0.02321 };
                case 110:
                    return new LMS { L = 1, M = 53.448, S = 0.02323 };
                case 111:
                    return new LMS { L = 1, M = 53.477, S = 0.02325 };
                case 112:
                    return new LMS { L = 1, M = 53.507, S = 0.02327 };
                case 113:
                    return new LMS { L = 1, M = 53.537, S = 0.02328 };
                case 114:
                    return new LMS { L = 1, M = 53.566, S = 0.0233 };
                case 115:
                    return new LMS { L = 1, M = 53.595, S = 0.02332 };
                case 116:
                    return new LMS { L = 1, M = 53.624, S = 0.02334 };
                case 117:
                    return new LMS { L = 1, M = 53.653, S = 0.02336 };
                case 118:
                    return new LMS { L = 1, M = 53.681, S = 0.02338 };
                case 119:
                    return new LMS { L = 1, M = 53.71, S = 0.0234 };
                case 120:
                    return new LMS { L = 1, M = 53.738, S = 0.02342 };
                case 121:
                    return new LMS { L = 1, M = 53.766, S = 0.02344 };
                case 122:
                    return new LMS { L = 1, M = 53.794, S = 0.02346 };
                case 123:
                    return new LMS { L = 1, M = 53.821, S = 0.02348 };
                case 124:
                    return new LMS { L = 1, M = 53.848, S = 0.0235 };
                case 125:
                    return new LMS { L = 1, M = 53.875, S = 0.02352 };
                case 126:
                    return new LMS { L = 1, M = 53.902, S = 0.02354 };
                case 127:
                    return new LMS { L = 1, M = 53.929, S = 0.02356 };
                case 128:
                    return new LMS { L = 1, M = 53.955, S = 0.02358 };
                case 129:
                    return new LMS { L = 1, M = 53.982, S = 0.0236 };
                case 130:
                    return new LMS { L = 1, M = 54.007, S = 0.02362 };
                case 131:
                    return new LMS { L = 1, M = 54.033, S = 0.02364 };
                case 132:
                    return new LMS { L = 1, M = 54.06, S = 0.02366 };
                case 133:
                    return new LMS { L = 1, M = 54.085, S = 0.02369 };
                case 134:
                    return new LMS { L = 1, M = 54.11, S = 0.02371 };
                case 135:
                    return new LMS { L = 1, M = 54.135, S = 0.02373 };
                case 136:
                    return new LMS { L = 1, M = 54.16, S = 0.02375 };
                case 137:
                    return new LMS { L = 1, M = 54.185, S = 0.02377 };
                case 138:
                    return new LMS { L = 1, M = 54.21, S = 0.02379 };
                case 139:
                    return new LMS { L = 1, M = 54.235, S = 0.02381 };
                case 140:
                    return new LMS { L = 1, M = 54.259, S = 0.02384 };
                case 141:
                    return new LMS { L = 1, M = 54.283, S = 0.02386 };
                case 142:
                    return new LMS { L = 1, M = 54.307, S = 0.02388 };
                case 143:
                    return new LMS { L = 1, M = 54.331, S = 0.0239 };
                case 144:
                    return new LMS { L = 1, M = 54.355, S = 0.02392 };
                case 145:
                    return new LMS { L = 1, M = 54.378, S = 0.02394 };
                case 146:
                    return new LMS { L = 1, M = 54.401, S = 0.02396 };
                case 147:
                    return new LMS { L = 1, M = 54.424, S = 0.02398 };
                case 148:
                    return new LMS { L = 1, M = 54.447, S = 0.024 };
                case 149:
                    return new LMS { L = 1, M = 54.47, S = 0.02403 };
                case 150:
                    return new LMS { L = 1, M = 54.492, S = 0.02405 };
                case 151:
                    return new LMS { L = 1, M = 54.515, S = 0.02407 };
                case 152:
                    return new LMS { L = 1, M = 54.537, S = 0.02409 };
                case 153:
                    return new LMS { L = 1, M = 54.559, S = 0.02411 };
                case 154:
                    return new LMS { L = 1, M = 54.581, S = 0.02413 };
                case 155:
                    return new LMS { L = 1, M = 54.603, S = 0.02415 };
                case 156:
                    return new LMS { L = 1, M = 54.624, S = 0.02417 };
                case 157:
                    return new LMS { L = 1, M = 54.646, S = 0.02419 };
                case 158:
                    return new LMS { L = 1, M = 54.667, S = 0.02421 };
                case 159:
                    return new LMS { L = 1, M = 54.688, S = 0.02423 };
                case 160:
                    return new LMS { L = 1, M = 54.71, S = 0.02425 };
                case 161:
                    return new LMS { L = 1, M = 54.731, S = 0.02427 };
                case 162:
                    return new LMS { L = 1, M = 54.751, S = 0.02429 };
                case 163:
                    return new LMS { L = 1, M = 54.772, S = 0.02431 };
                case 164:
                    return new LMS { L = 1, M = 54.793, S = 0.02433 };
                case 165:
                    return new LMS { L = 1, M = 54.813, S = 0.02435 };
                case 166:
                    return new LMS { L = 1, M = 54.833, S = 0.02437 };
                case 167:
                    return new LMS { L = 1, M = 54.853, S = 0.02439 };
                case 168:
                    return new LMS { L = 1, M = 54.873, S = 0.02441 };
                case 169:
                    return new LMS { L = 1, M = 54.894, S = 0.02442 };
                case 170:
                    return new LMS { L = 1, M = 54.913, S = 0.02444 };
                case 171:
                    return new LMS { L = 1, M = 54.933, S = 0.02446 };
                case 172:
                    return new LMS { L = 1, M = 54.953, S = 0.02448 };
                case 173:
                    return new LMS { L = 1, M = 54.972, S = 0.0245 };
                case 174:
                    return new LMS { L = 1, M = 54.992, S = 0.02452 };
                case 175:
                    return new LMS { L = 1, M = 55.011, S = 0.02454 };
                case 176:
                    return new LMS { L = 1, M = 55.03, S = 0.02455 };
                case 177:
                    return new LMS { L = 1, M = 55.049, S = 0.02457 };
                case 178:
                    return new LMS { L = 1, M = 55.068, S = 0.02459 };
                case 179:
                    return new LMS { L = 1, M = 55.087, S = 0.02461 };
                case 180:
                    return new LMS { L = 1, M = 55.106, S = 0.02462 };
                case 181:
                    return new LMS { L = 1, M = 55.124, S = 0.02464 };
                case 182:
                    return new LMS { L = 1, M = 55.142, S = 0.02466 };
                case 183:
                    return new LMS { L = 1, M = 55.161, S = 0.02468 };
                case 184:
                    return new LMS { L = 1, M = 55.179, S = 0.0247 };
                case 185:
                    return new LMS { L = 1, M = 55.197, S = 0.02471 };
                case 186:
                    return new LMS { L = 1, M = 55.215, S = 0.02473 };
                case 187:
                    return new LMS { L = 1, M = 55.234, S = 0.02475 };
                case 188:
                    return new LMS { L = 1, M = 55.251, S = 0.02476 };
                case 189:
                    return new LMS { L = 1, M = 55.269, S = 0.02478 };
                case 190:
                    return new LMS { L = 1, M = 55.286, S = 0.0248 };
                case 191:
                    return new LMS { L = 1, M = 55.304, S = 0.02481 };
                case 192:
                    return new LMS { L = 1, M = 55.321, S = 0.02483 };
                case 193:
                    return new LMS { L = 1, M = 55.339, S = 0.02485 };
                case 194:
                    return new LMS { L = 1, M = 55.355, S = 0.02486 };
                case 195:
                    return new LMS { L = 1, M = 55.373, S = 0.02488 };
                case 196:
                    return new LMS { L = 1, M = 55.39, S = 0.02489 };
                case 197:
                    return new LMS { L = 1, M = 55.406, S = 0.02491 };
                case 198:
                    return new LMS { L = 1, M = 55.424, S = 0.02493 };
                case 199:
                    return new LMS { L = 1, M = 55.441, S = 0.02494 };
                case 200:
                    return new LMS { L = 1, M = 55.457, S = 0.02496 };
                case 201:
                    return new LMS { L = 1, M = 55.474, S = 0.02497 };
                case 202:
                    return new LMS { L = 1, M = 55.491, S = 0.02499 };
                case 203:
                    return new LMS { L = 1, M = 55.506, S = 0.02501 };
                case 204:
                    return new LMS { L = 1, M = 55.52, S = 0.02501 };
                default:
                    throw new ArgumentOutOfRangeException("ageMonths");
            }
        }
    }
}

