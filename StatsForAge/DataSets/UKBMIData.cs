using System;
namespace StatsForAge.DataSets
{
    public sealed class UKBMIData : CentileData
    {
        internal UKBMIData()
        {
            base.GestAgeRange = new GenderRange(43, 43);
        }
        protected override LMS LMSForGestAge(int gestAgeWeeks, bool isMale)
        {
            if (isMale)
            {
                switch (gestAgeWeeks)
                {
                    case 43:
                        return new LMS { L = 0.3449, M = 14.2241, S = 0.0923 };
                    default:
                        throw new ArgumentOutOfRangeException("gestAgeWeeks");
                }
            }
            switch (gestAgeWeeks)
            {
                //Female
                case 43:
                    return new LMS { L = 0.4263, M = 13.9505, S = 0.09647 };
                default:
                    throw new ArgumentOutOfRangeException("gestAgeWeeks");
            }
        }

        protected override LMS LMSForAgeWeeks(int ageWeeks, bool isMale)
        {
            if (isMale)
            {
                switch (ageWeeks)
                {
                    case 4:
                        return new LMS { L = 0.2881, M = 14.7714, S = 0.09072 };
                    case 5:
                        return new LMS { L = 0.2409, M = 15.2355, S = 0.08953 };
                    case 6:
                        return new LMS { L = 0.2003, M = 15.6107, S = 0.08859 };
                    case 7:
                        return new LMS { L = 0.1645, M = 15.9169, S = 0.08782 };
                    case 8:
                        return new LMS { L = 0.1324, M = 16.1698, S = 0.08717 };
                    case 9:
                        return new LMS { L = 0.1032, M = 16.3787, S = 0.08661 };
                    case 10:
                        return new LMS { L = 0.0766, M = 16.5494, S = 0.08612 };
                    case 11:
                        return new LMS { L = 0.052, M = 16.6882, S = 0.08569 };
                    case 12:
                        return new LMS { L = 0.0291, M = 16.8016, S = 0.08531 };
                    case 13:
                        return new LMS { L = 0.0077, M = 16.895, S = 0.08496 };
                    default:
                        throw new ArgumentOutOfRangeException("ageWeeks");
                }
            }
            switch (ageWeeks)
            {
                case 4:
                    return new LMS { L = 0.3637, M = 14.4208, S = 0.09577 };
                case 5:
                    return new LMS { L = 0.3124, M = 14.8157, S = 0.0952 };
                case 6:
                    return new LMS { L = 0.2688, M = 15.138, S = 0.09472 };
                case 7:
                    return new LMS { L = 0.2306, M = 15.4063, S = 0.09431 };
                case 8:
                    return new LMS { L = 0.1966, M = 15.6311, S = 0.09394 };
                case 9:
                    return new LMS { L = 0.1658, M = 15.8232, S = 0.09361 };
                case 10:
                    return new LMS { L = 0.1377, M = 15.9874, S = 0.09332 };
                case 11:
                    return new LMS { L = 0.1118, M = 16.1277, S = 0.09304 };
                case 12:
                    return new LMS { L = 0.0877, M = 16.2485, S = 0.09279 };
                case 13:
                    return new LMS { L = 0.0652, M = 16.3531, S = 0.09255 };
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
                        return new LMS { L = 0.0068, M = 16.8987, S = 0.08495 };
                    case 4:
                        return new LMS { L = -0.0727, M = 17.1579, S = 0.08378 };
                    case 5:
                        return new LMS { L = -0.137, M = 17.2919, S = 0.08296 };
                    case 6:
                        return new LMS { L = -0.1913, M = 17.3422, S = 0.08234 };
                    case 7:
                        return new LMS { L = -0.2385, M = 17.3288, S = 0.08183 };
                    case 8:
                        return new LMS { L = -0.2802, M = 17.2647, S = 0.0814 };
                    case 9:
                        return new LMS { L = -0.3176, M = 17.1662, S = 0.08102 };
                    case 10:
                        return new LMS { L = -0.3516, M = 17.0488, S = 0.08068 };
                    case 11:
                        return new LMS { L = -0.3828, M = 16.9239, S = 0.08037 };
                    case 12:
                        return new LMS { L = -0.4115, M = 16.7981, S = 0.08009 };
                    case 13:
                        return new LMS { L = -0.4382, M = 16.6743, S = 0.07982 };
                    case 14:
                        return new LMS { L = -0.463, M = 16.5548, S = 0.07958 };
                    case 15:
                        return new LMS { L = -0.4863, M = 16.4409, S = 0.07935 };
                    case 16:
                        return new LMS { L = -0.5082, M = 16.3335, S = 0.07913 };
                    case 17:
                        return new LMS { L = -0.5289, M = 16.2329, S = 0.07892 };
                    case 18:
                        return new LMS { L = -0.5484, M = 16.1392, S = 0.07873 };
                    case 19:
                        return new LMS { L = -0.5669, M = 16.0528, S = 0.07854 };
                    case 20:
                        return new LMS { L = -0.5846, M = 15.9743, S = 0.07836 };
                    case 21:
                        return new LMS { L = -0.6014, M = 15.9039, S = 0.07818 };
                    case 22:
                        return new LMS { L = -0.6174, M = 15.8412, S = 0.07802 };
                    case 23:
                        return new LMS { L = -0.6328, M = 15.7852, S = 0.07786 };
                    case 24:
                        return new LMS { L = -0.6473, M = 15.7356, S = 0.07771 };
                    case 25:
                        return new LMS { L = -0.584, M = 15.98, S = 0.07792 };
                    case 26:
                        return new LMS { L = -0.5497, M = 15.9414, S = 0.078 };
                    case 27:
                        return new LMS { L = -0.5166, M = 15.9036, S = 0.07808 };
                    case 28:
                        return new LMS { L = -0.485, M = 15.8667, S = 0.07818 };
                    case 29:
                        return new LMS { L = -0.4552, M = 15.8306, S = 0.07829 };
                    case 30:
                        return new LMS { L = -0.4274, M = 15.7953, S = 0.07841 };
                    case 31:
                        return new LMS { L = -0.4016, M = 15.7606, S = 0.07854 };
                    case 32:
                        return new LMS { L = -0.3782, M = 15.7267, S = 0.07867 };
                    case 33:
                        return new LMS { L = -0.3572, M = 15.6934, S = 0.07882 };
                    case 34:
                        return new LMS { L = -0.3388, M = 15.661, S = 0.07897 };
                    case 35:
                        return new LMS { L = -0.3231, M = 15.6294, S = 0.07914 };
                    case 36:
                        return new LMS { L = -0.3101, M = 15.5988, S = 0.07931 };
                    case 37:
                        return new LMS { L = -0.3, M = 15.5693, S = 0.0795 };
                    case 38:
                        return new LMS { L = -0.2927, M = 15.541, S = 0.07969 };
                    case 39:
                        return new LMS { L = -0.2884, M = 15.514, S = 0.0799 };
                    case 40:
                        return new LMS { L = -0.2869, M = 15.4885, S = 0.08012 };
                    case 41:
                        return new LMS { L = -0.2881, M = 15.4645, S = 0.08036 };
                    case 42:
                        return new LMS { L = -0.2919, M = 15.442, S = 0.08061 };
                    case 43:
                        return new LMS { L = -0.2981, M = 15.421, S = 0.08087 };
                    case 44:
                        return new LMS { L = -0.3067, M = 15.4013, S = 0.08115 };
                    case 45:
                        return new LMS { L = -0.3174, M = 15.3827, S = 0.08144 };
                    case 46:
                        return new LMS { L = -0.3303, M = 15.3652, S = 0.08174 };
                    case 47:
                        return new LMS { L = -0.3452, M = 15.3485, S = 0.08205 };
                    case 48:
                        return new LMS { L = -0.3622, M = 15.3326, S = 0.08238 };
                    case 49:
                        return new LMS { L = -1.291, M = 15.752, S = 0.07684 };
                    case 50:
                        return new LMS { L = -1.325, M = 15.704, S = 0.07692 };
                    case 51:
                        return new LMS { L = -1.342, M = 15.682, S = 0.077 };
                    case 52:
                        return new LMS { L = -1.359, M = 15.662, S = 0.07709 };
                    case 53:
                        return new LMS { L = -1.375, M = 15.644, S = 0.0772 };
                    case 54:
                        return new LMS { L = -1.391, M = 15.626, S = 0.07733 };
                    case 55:
                        return new LMS { L = -1.407, M = 15.61, S = 0.07748 };
                    case 56:
                        return new LMS { L = -1.422, M = 15.595, S = 0.07765 };
                    case 57:
                        return new LMS { L = -1.437, M = 15.582, S = 0.07784 };
                    case 58:
                        return new LMS { L = -1.452, M = 15.569, S = 0.07806 };
                    case 59:
                        return new LMS { L = -1.467, M = 15.557, S = 0.07829 };
                    case 60:
                        return new LMS { L = -1.481, M = 15.547, S = 0.07856 };
                    case 61:
                        return new LMS { L = -1.495, M = 15.538, S = 0.07884 };
                    case 62:
                        return new LMS { L = -1.509, M = 15.53, S = 0.07915 };
                    case 63:
                        return new LMS { L = -1.523, M = 15.523, S = 0.07948 };
                    case 64:
                        return new LMS { L = -1.536, M = 15.517, S = 0.07983 };
                    case 65:
                        return new LMS { L = -1.549, M = 15.511, S = 0.0802 };
                    case 66:
                        return new LMS { L = -1.562, M = 15.507, S = 0.08059 };
                    case 67:
                        return new LMS { L = -1.575, M = 15.503, S = 0.081 };
                    case 68:
                        return new LMS { L = -1.587, M = 15.5, S = 0.08143 };
                    case 69:
                        return new LMS { L = -1.599, M = 15.498, S = 0.08189 };
                    case 70:
                        return new LMS { L = -1.611, M = 15.497, S = 0.08235 };
                    case 71:
                        return new LMS { L = -1.622, M = 15.497, S = 0.08284 };
                    case 72:
                        return new LMS { L = -1.634, M = 15.498, S = 0.08334 };
                    case 73:
                        return new LMS { L = -1.644, M = 15.499, S = 0.08386 };
                    case 74:
                        return new LMS { L = -1.655, M = 15.501, S = 0.08439 };
                    case 75:
                        return new LMS { L = -1.665, M = 15.503, S = 0.08494 };
                    case 76:
                        return new LMS { L = -1.675, M = 15.507, S = 0.08549 };
                    case 77:
                        return new LMS { L = -1.685, M = 15.511, S = 0.08606 };
                    case 78:
                        return new LMS { L = -1.694, M = 15.516, S = 0.08663 };
                    case 79:
                        return new LMS { L = -1.704, M = 15.522, S = 0.08722 };
                    case 80:
                        return new LMS { L = -1.713, M = 15.529, S = 0.08781 };
                    case 81:
                        return new LMS { L = -1.721, M = 15.536, S = 0.08841 };
                    case 82:
                        return new LMS { L = -1.73, M = 15.545, S = 0.08901 };
                    case 83:
                        return new LMS { L = -1.738, M = 15.554, S = 0.08962 };
                    case 84:
                        return new LMS { L = -1.745, M = 15.564, S = 0.09023 };
                    case 85:
                        return new LMS { L = -1.753, M = 15.575, S = 0.09084 };
                    case 86:
                        return new LMS { L = -1.76, M = 15.587, S = 0.09145 };
                    case 87:
                        return new LMS { L = -1.767, M = 15.6, S = 0.09207 };
                    case 88:
                        return new LMS { L = -1.774, M = 15.614, S = 0.09268 };
                    case 89:
                        return new LMS { L = -1.781, M = 15.628, S = 0.0933 };
                    case 90:
                        return new LMS { L = -1.787, M = 15.643, S = 0.09391 };
                    case 91:
                        return new LMS { L = -1.793, M = 15.659, S = 0.09451 };
                    case 92:
                        return new LMS { L = -1.798, M = 15.675, S = 0.09512 };
                    case 93:
                        return new LMS { L = -1.804, M = 15.692, S = 0.09572 };
                    case 94:
                        return new LMS { L = -1.809, M = 15.71, S = 0.09632 };
                    case 95:
                        return new LMS { L = -1.814, M = 15.729, S = 0.09691 };
                    case 96:
                        return new LMS { L = -1.818, M = 15.748, S = 0.09749 };
                    case 97:
                        return new LMS { L = -1.823, M = 15.768, S = 0.09807 };
                    case 98:
                        return new LMS { L = -1.827, M = 15.789, S = 0.09864 };
                    case 99:
                        return new LMS { L = -1.83, M = 15.81, S = 0.0992 };
                    case 100:
                        return new LMS { L = -1.834, M = 15.833, S = 0.09976 };
                    case 101:
                        return new LMS { L = -1.837, M = 15.855, S = 0.1003 };
                    case 102:
                        return new LMS { L = -1.84, M = 15.88, S = 0.10084 };
                    case 103:
                        return new LMS { L = -1.843, M = 15.904, S = 0.10137 };
                    case 104:
                        return new LMS { L = -1.846, M = 15.929, S = 0.10189 };
                    case 105:
                        return new LMS { L = -1.848, M = 15.955, S = 0.1024 };
                    case 106:
                        return new LMS { L = -1.85, M = 15.982, S = 0.1029 };
                    case 107:
                        return new LMS { L = -1.852, M = 16.009, S = 0.1034 };
                    case 108:
                        return new LMS { L = -1.854, M = 16.037, S = 0.10388 };
                    case 109:
                        return new LMS { L = -1.855, M = 16.066, S = 0.10435 };
                    case 110:
                        return new LMS { L = -1.856, M = 16.095, S = 0.10482 };
                    case 111:
                        return new LMS { L = -1.857, M = 16.125, S = 0.10527 };
                    case 112:
                        return new LMS { L = -1.858, M = 16.155, S = 0.10571 };
                    case 113:
                        return new LMS { L = -1.858, M = 16.187, S = 0.10615 };
                    case 114:
                        return new LMS { L = -1.859, M = 16.219, S = 0.10657 };
                    case 115:
                        return new LMS { L = -1.859, M = 16.251, S = 0.10698 };
                    case 116:
                        return new LMS { L = -1.859, M = 16.284, S = 0.10738 };
                    case 117:
                        return new LMS { L = -1.859, M = 16.318, S = 0.10777 };
                    case 118:
                        return new LMS { L = -1.859, M = 16.352, S = 0.10815 };
                    case 119:
                        return new LMS { L = -1.858, M = 16.387, S = 0.10852 };
                    case 120:
                        return new LMS { L = -1.857, M = 16.423, S = 0.10888 };
                    case 121:
                        return new LMS { L = -1.856, M = 16.459, S = 0.10923 };
                    case 122:
                        return new LMS { L = -1.855, M = 16.496, S = 0.10957 };
                    case 123:
                        return new LMS { L = -1.854, M = 16.533, S = 0.1099 };
                    case 124:
                        return new LMS { L = -1.853, M = 16.57, S = 0.11022 };
                    case 125:
                        return new LMS { L = -1.851, M = 16.609, S = 0.11054 };
                    case 126:
                        return new LMS { L = -1.85, M = 16.648, S = 0.11084 };
                    case 127:
                        return new LMS { L = -1.848, M = 16.687, S = 0.11114 };
                    case 128:
                        return new LMS { L = -1.846, M = 16.727, S = 0.11143 };
                    case 129:
                        return new LMS { L = -1.844, M = 16.768, S = 0.1117 };
                    case 130:
                        return new LMS { L = -1.842, M = 16.808, S = 0.11197 };
                    case 131:
                        return new LMS { L = -1.839, M = 16.85, S = 0.11223 };
                    case 132:
                        return new LMS { L = -1.837, M = 16.892, S = 0.11249 };
                    case 133:
                        return new LMS { L = -1.834, M = 16.935, S = 0.11273 };
                    case 134:
                        return new LMS { L = -1.831, M = 16.977, S = 0.11296 };
                    case 135:
                        return new LMS { L = -1.829, M = 17.02, S = 0.11319 };
                    case 136:
                        return new LMS { L = -1.826, M = 17.065, S = 0.11341 };
                    case 137:
                        return new LMS { L = -1.823, M = 17.108, S = 0.11362 };
                    case 138:
                        return new LMS { L = -1.819, M = 17.154, S = 0.11382 };
                    case 139:
                        return new LMS { L = -1.816, M = 17.199, S = 0.11402 };
                    case 140:
                        return new LMS { L = -1.813, M = 17.244, S = 0.1142 };
                    case 141:
                        return new LMS { L = -1.809, M = 17.291, S = 0.11438 };
                    case 142:
                        return new LMS { L = -1.806, M = 17.338, S = 0.11456 };
                    case 143:
                        return new LMS { L = -1.802, M = 17.386, S = 0.11472 };
                    case 144:
                        return new LMS { L = -1.799, M = 17.433, S = 0.11488 };
                    case 145:
                        return new LMS { L = -1.795, M = 17.481, S = 0.11503 };
                    case 146:
                        return new LMS { L = -1.791, M = 17.53, S = 0.11517 };
                    case 147:
                        return new LMS { L = -1.787, M = 17.579, S = 0.11532 };
                    case 148:
                        return new LMS { L = -1.783, M = 17.629, S = 0.11545 };
                    case 149:
                        return new LMS { L = -1.78, M = 17.679, S = 0.11558 };
                    case 150:
                        return new LMS { L = -1.776, M = 17.729, S = 0.1157 };
                    case 151:
                        return new LMS { L = -1.771, M = 17.779, S = 0.11581 };
                    case 152:
                        return new LMS { L = -1.767, M = 17.83, S = 0.11592 };
                    case 153:
                        return new LMS { L = -1.763, M = 17.881, S = 0.11603 };
                    case 154:
                        return new LMS { L = -1.759, M = 17.933, S = 0.11613 };
                    case 155:
                        return new LMS { L = -1.755, M = 17.985, S = 0.11622 };
                    case 156:
                        return new LMS { L = -1.75, M = 18.037, S = 0.11631 };
                    case 157:
                        return new LMS { L = -1.746, M = 18.089, S = 0.11639 };
                    case 158:
                        return new LMS { L = -1.742, M = 18.142, S = 0.11647 };
                    case 159:
                        return new LMS { L = -1.738, M = 18.194, S = 0.11655 };
                    case 160:
                        return new LMS { L = -1.733, M = 18.247, S = 0.11662 };
                    case 161:
                        return new LMS { L = -1.729, M = 18.3, S = 0.11668 };
                    case 162:
                        return new LMS { L = -1.724, M = 18.354, S = 0.11674 };
                    case 163:
                        return new LMS { L = -1.72, M = 18.407, S = 0.1168 };
                    case 164:
                        return new LMS { L = -1.715, M = 18.46, S = 0.11685 };
                    case 165:
                        return new LMS { L = -1.711, M = 18.514, S = 0.1169 };
                    case 166:
                        return new LMS { L = -1.707, M = 18.567, S = 0.11695 };
                    case 167:
                        return new LMS { L = -1.702, M = 18.621, S = 0.11699 };
                    case 168:
                        return new LMS { L = -1.697, M = 18.675, S = 0.11703 };
                    case 169:
                        return new LMS { L = -1.693, M = 18.729, S = 0.11706 };
                    case 170:
                        return new LMS { L = -1.689, M = 18.783, S = 0.1171 };
                    case 171:
                        return new LMS { L = -1.684, M = 18.836, S = 0.11712 };
                    case 172:
                        return new LMS { L = -1.68, M = 18.89, S = 0.11715 };
                    case 173:
                        return new LMS { L = -1.675, M = 18.944, S = 0.11717 };
                    case 174:
                        return new LMS { L = -1.671, M = 18.997, S = 0.11719 };
                    case 175:
                        return new LMS { L = -1.666, M = 19.051, S = 0.11721 };
                    case 176:
                        return new LMS { L = -1.661, M = 19.104, S = 0.11722 };
                    case 177:
                        return new LMS { L = -1.657, M = 19.158, S = 0.11723 };
                    case 178:
                        return new LMS { L = -1.652, M = 19.211, S = 0.11724 };
                    case 179:
                        return new LMS { L = -1.648, M = 19.264, S = 0.11724 };
                    case 180:
                        return new LMS { L = -1.643, M = 19.317, S = 0.11725 };
                    case 181:
                        return new LMS { L = -1.639, M = 19.37, S = 0.11725 };
                    case 182:
                        return new LMS { L = -1.635, M = 19.423, S = 0.11725 };
                    case 183:
                        return new LMS { L = -1.63, M = 19.475, S = 0.11724 };
                    case 184:
                        return new LMS { L = -1.626, M = 19.528, S = 0.11724 };
                    case 185:
                        return new LMS { L = -1.621, M = 19.579, S = 0.11723 };
                    case 186:
                        return new LMS { L = -1.617, M = 19.632, S = 0.11722 };
                    case 187:
                        return new LMS { L = -1.612, M = 19.683, S = 0.11721 };
                    case 188:
                        return new LMS { L = -1.608, M = 19.735, S = 0.11719 };
                    case 189:
                        return new LMS { L = -1.603, M = 19.786, S = 0.11718 };
                    case 190:
                        return new LMS { L = -1.599, M = 19.837, S = 0.11716 };
                    case 191:
                        return new LMS { L = -1.595, M = 19.887, S = 0.11714 };
                    case 192:
                        return new LMS { L = -1.59, M = 19.938, S = 0.11712 };
                    case 193:
                        return new LMS { L = -1.586, M = 19.988, S = 0.1171 };
                    case 194:
                        return new LMS { L = -1.582, M = 20.038, S = 0.11708 };
                    case 195:
                        return new LMS { L = -1.577, M = 20.087, S = 0.11706 };
                    case 196:
                        return new LMS { L = -1.573, M = 20.137, S = 0.11703 };
                    case 197:
                        return new LMS { L = -1.569, M = 20.186, S = 0.117 };
                    case 198:
                        return new LMS { L = -1.564, M = 20.234, S = 0.11698 };
                    case 199:
                        return new LMS { L = -1.56, M = 20.282, S = 0.11695 };
                    case 200:
                        return new LMS { L = -1.556, M = 20.33, S = 0.11692 };
                    case 201:
                        return new LMS { L = -1.551, M = 20.378, S = 0.11689 };
                    case 202:
                        return new LMS { L = -1.547, M = 20.425, S = 0.11686 };
                    case 203:
                        return new LMS { L = -1.543, M = 20.472, S = 0.11683 };
                    case 204:
                        return new LMS { L = -1.538, M = 20.519, S = 0.1168 };
                    case 205:
                        return new LMS { L = -1.534, M = 20.565, S = 0.11677 };
                    case 206:
                        return new LMS { L = -1.53, M = 20.611, S = 0.11674 };
                    case 207:
                        return new LMS { L = -1.526, M = 20.656, S = 0.1167 };
                    case 208:
                        return new LMS { L = -1.521, M = 20.702, S = 0.11667 };
                    case 209:
                        return new LMS { L = -1.517, M = 20.746, S = 0.11663 };
                    case 210:
                        return new LMS { L = -1.513, M = 20.791, S = 0.1166 };
                    case 211:
                        return new LMS { L = -1.509, M = 20.836, S = 0.11657 };
                    case 212:
                        return new LMS { L = -1.505, M = 20.879, S = 0.11653 };
                    case 213:
                        return new LMS { L = -1.501, M = 20.923, S = 0.11649 };
                    case 214:
                        return new LMS { L = -1.496, M = 20.967, S = 0.11646 };
                    case 215:
                        return new LMS { L = -1.492, M = 21.009, S = 0.11642 };
                    case 216:
                        return new LMS { L = -1.488, M = 21.052, S = 0.11639 };
                    case 217:
                        return new LMS { L = -1.484, M = 21.095, S = 0.11635 };
                    case 218:
                        return new LMS { L = -1.48, M = 21.136, S = 0.11631 };
                    case 219:
                        return new LMS { L = -1.476, M = 21.178, S = 0.11628 };
                    case 220:
                        return new LMS { L = -1.472, M = 21.22, S = 0.11624 };
                    case 221:
                        return new LMS { L = -1.467, M = 21.26, S = 0.1162 };
                    case 222:
                        return new LMS { L = -1.463, M = 21.301, S = 0.11617 };
                    case 223:
                        return new LMS { L = -1.459, M = 21.342, S = 0.11613 };
                    case 224:
                        return new LMS { L = -1.455, M = 21.382, S = 0.11609 };
                    case 225:
                        return new LMS { L = -1.451, M = 21.422, S = 0.11605 };
                    case 226:
                        return new LMS { L = -1.447, M = 21.461, S = 0.11602 };
                    case 227:
                        return new LMS { L = -1.443, M = 21.501, S = 0.11598 };
                    case 228:
                        return new LMS { L = -1.439, M = 21.54, S = 0.11594 };
                    case 229:
                        return new LMS { L = -1.435, M = 21.578, S = 0.11591 };
                    case 230:
                        return new LMS { L = -1.431, M = 21.617, S = 0.11587 };
                    case 231:
                        return new LMS { L = -1.427, M = 21.655, S = 0.11583 };
                    case 232:
                        return new LMS { L = -1.423, M = 21.693, S = 0.1158 };
                    case 233:
                        return new LMS { L = -1.419, M = 21.73, S = 0.11576 };
                    case 234:
                        return new LMS { L = -1.415, M = 21.768, S = 0.11572 };
                    case 235:
                        return new LMS { L = -1.412, M = 21.805, S = 0.11569 };
                    case 236:
                        return new LMS { L = -1.408, M = 21.842, S = 0.11565 };
                    case 237:
                        return new LMS { L = -1.404, M = 21.878, S = 0.11561 };
                    case 238:
                        return new LMS { L = -1.4, M = 21.914, S = 0.11558 };
                    case 239:
                        return new LMS { L = -1.396, M = 21.951, S = 0.11554 };
                    case 240:
                        return new LMS { L = -1.392, M = 21.986, S = 0.11551 };
                    default:
                        throw new ArgumentOutOfRangeException("ageMonths");
                }
            }
            switch (ageMonths)
            {
                case 3:
                    return new LMS { L = 0.0643, M = 16.3574, S = 0.09254 };
                case 4:
                    return new LMS { L = -0.0191, M = 16.6703, S = 0.09166 };
                case 5:
                    return new LMS { L = -0.0864, M = 16.8386, S = 0.09096 };
                case 6:
                    return new LMS { L = -0.1429, M = 16.9083, S = 0.09036 };
                case 7:
                    return new LMS { L = -0.1916, M = 16.902, S = 0.08984 };
                case 8:
                    return new LMS { L = -0.2344, M = 16.8404, S = 0.08939 };
                case 9:
                    return new LMS { L = -0.2725, M = 16.7406, S = 0.08898 };
                case 10:
                    return new LMS { L = -0.3068, M = 16.6184, S = 0.08861 };
                case 11:
                    return new LMS { L = -0.3381, M = 16.4875, S = 0.08828 };
                case 12:
                    return new LMS { L = -0.3667, M = 16.3568, S = 0.08797 };
                case 13:
                    return new LMS { L = -0.3932, M = 16.2311, S = 0.08768 };
                case 14:
                    return new LMS { L = -0.4177, M = 16.1128, S = 0.08741 };
                case 15:
                    return new LMS { L = -0.4407, M = 16.0028, S = 0.08716 };
                case 16:
                    return new LMS { L = -0.4623, M = 15.9017, S = 0.08693 };
                case 17:
                    return new LMS { L = -0.4825, M = 15.8096, S = 0.08671 };
                case 18:
                    return new LMS { L = -0.5017, M = 15.7263, S = 0.0865 };
                case 19:
                    return new LMS { L = -0.5199, M = 15.6517, S = 0.0863 };
                case 20:
                    return new LMS { L = -0.5372, M = 15.5855, S = 0.08612 };
                case 21:
                    return new LMS { L = -0.5537, M = 15.5278, S = 0.08594 };
                case 22:
                    return new LMS { L = -0.5695, M = 15.4787, S = 0.08577 };
                case 23:
                    return new LMS { L = -0.5846, M = 15.438, S = 0.0856 };
                case 24:
                    return new LMS { L = -0.5989, M = 15.4052, S = 0.08545 };
                case 25:
                    return new LMS { L = -0.5684, M = 15.659, S = 0.08452 };
                case 26:
                    return new LMS { L = -0.5684, M = 15.6308, S = 0.08449 };
                case 27:
                    return new LMS { L = -0.5684, M = 15.6037, S = 0.08446 };
                case 28:
                    return new LMS { L = -0.5684, M = 15.5777, S = 0.08444 };
                case 29:
                    return new LMS { L = -0.5684, M = 15.5523, S = 0.08443 };
                case 30:
                    return new LMS { L = -0.5684, M = 15.5276, S = 0.08444 };
                case 31:
                    return new LMS { L = -0.5684, M = 15.5034, S = 0.08448 };
                case 32:
                    return new LMS { L = -0.5684, M = 15.4798, S = 0.08455 };
                case 33:
                    return new LMS { L = -0.5684, M = 15.4572, S = 0.08467 };
                case 34:
                    return new LMS { L = -0.5684, M = 15.4356, S = 0.08484 };
                case 35:
                    return new LMS { L = -0.5684, M = 15.4155, S = 0.08506 };
                case 36:
                    return new LMS { L = -0.5684, M = 15.3968, S = 0.08535 };
                case 37:
                    return new LMS { L = -0.5684, M = 15.3796, S = 0.08569 };
                case 38:
                    return new LMS { L = -0.5684, M = 15.3638, S = 0.08609 };
                case 39:
                    return new LMS { L = -0.5684, M = 15.3493, S = 0.08654 };
                case 40:
                    return new LMS { L = -0.5684, M = 15.3358, S = 0.08704 };
                case 41:
                    return new LMS { L = -0.5684, M = 15.3233, S = 0.08757 };
                case 42:
                    return new LMS { L = -0.5684, M = 15.3116, S = 0.08813 };
                case 43:
                    return new LMS { L = -0.5684, M = 15.3007, S = 0.08872 };
                case 44:
                    return new LMS { L = -0.5684, M = 15.2905, S = 0.08931 };
                case 45:
                    return new LMS { L = -0.5684, M = 15.2814, S = 0.08991 };
                case 46:
                    return new LMS { L = -0.5684, M = 15.2732, S = 0.09051 };
                case 47:
                    return new LMS { L = -0.5684, M = 15.2661, S = 0.0911 };
                case 48:
                    return new LMS { L = -0.5684, M = 15.2602, S = 0.09168 };
                case 49:
                    return new LMS { L = -1.151, M = 15.656, S = 0.08728 };
                case 50:
                    return new LMS { L = -1.163, M = 15.622, S = 0.08814 };
                case 51:
                    return new LMS { L = -1.169, M = 15.605, S = 0.0886 };
                case 52:
                    return new LMS { L = -1.175, M = 15.589, S = 0.08906 };
                case 53:
                    return new LMS { L = -1.181, M = 15.573, S = 0.08954 };
                case 54:
                    return new LMS { L = -1.187, M = 15.557, S = 0.09004 };
                case 55:
                    return new LMS { L = -1.193, M = 15.542, S = 0.09054 };
                case 56:
                    return new LMS { L = -1.198, M = 15.528, S = 0.09106 };
                case 57:
                    return new LMS { L = -1.204, M = 15.515, S = 0.0916 };
                case 58:
                    return new LMS { L = -1.209, M = 15.503, S = 0.09214 };
                case 59:
                    return new LMS { L = -1.215, M = 15.492, S = 0.0927 };
                case 60:
                    return new LMS { L = -1.22, M = 15.483, S = 0.09326 };
                case 61:
                    return new LMS { L = -1.225, M = 15.475, S = 0.09384 };
                case 62:
                    return new LMS { L = -1.231, M = 15.468, S = 0.09443 };
                case 63:
                    return new LMS { L = -1.236, M = 15.463, S = 0.09503 };
                case 64:
                    return new LMS { L = -1.241, M = 15.46, S = 0.09563 };
                case 65:
                    return new LMS { L = -1.245, M = 15.457, S = 0.09624 };
                case 66:
                    return new LMS { L = -1.25, M = 15.457, S = 0.09686 };
                case 67:
                    return new LMS { L = -1.255, M = 15.458, S = 0.09749 };
                case 68:
                    return new LMS { L = -1.26, M = 15.461, S = 0.09812 };
                case 69:
                    return new LMS { L = -1.264, M = 15.465, S = 0.09875 };
                case 70:
                    return new LMS { L = -1.269, M = 15.47, S = 0.0994 };
                case 71:
                    return new LMS { L = -1.273, M = 15.477, S = 0.10004 };
                case 72:
                    return new LMS { L = -1.277, M = 15.485, S = 0.10069 };
                case 73:
                    return new LMS { L = -1.281, M = 15.494, S = 0.10135 };
                case 74:
                    return new LMS { L = -1.286, M = 15.506, S = 0.102 };
                case 75:
                    return new LMS { L = -1.289, M = 15.517, S = 0.10266 };
                case 76:
                    return new LMS { L = -1.293, M = 15.53, S = 0.10332 };
                case 77:
                    return new LMS { L = -1.297, M = 15.544, S = 0.10397 };
                case 78:
                    return new LMS { L = -1.301, M = 15.56, S = 0.10463 };
                case 79:
                    return new LMS { L = -1.304, M = 15.577, S = 0.10529 };
                case 80:
                    return new LMS { L = -1.308, M = 15.596, S = 0.10595 };
                case 81:
                    return new LMS { L = -1.311, M = 15.614, S = 0.1066 };
                case 82:
                    return new LMS { L = -1.314, M = 15.635, S = 0.10725 };
                case 83:
                    return new LMS { L = -1.317, M = 15.656, S = 0.10789 };
                case 84:
                    return new LMS { L = -1.32, M = 15.677, S = 0.10854 };
                case 85:
                    return new LMS { L = -1.323, M = 15.7, S = 0.10918 };
                case 86:
                    return new LMS { L = -1.325, M = 15.723, S = 0.10981 };
                case 87:
                    return new LMS { L = -1.328, M = 15.748, S = 0.11044 };
                case 88:
                    return new LMS { L = -1.33, M = 15.772, S = 0.11106 };
                case 89:
                    return new LMS { L = -1.332, M = 15.798, S = 0.11167 };
                case 90:
                    return new LMS { L = -1.334, M = 15.824, S = 0.11228 };
                case 91:
                    return new LMS { L = -1.336, M = 15.85, S = 0.11288 };
                case 92:
                    return new LMS { L = -1.338, M = 15.877, S = 0.11346 };
                case 93:
                    return new LMS { L = -1.339, M = 15.905, S = 0.11404 };
                case 94:
                    return new LMS { L = -1.341, M = 15.934, S = 0.11461 };
                case 95:
                    return new LMS { L = -1.342, M = 15.963, S = 0.11517 };
                case 96:
                    return new LMS { L = -1.344, M = 15.993, S = 0.11572 };
                case 97:
                    return new LMS { L = -1.345, M = 16.022, S = 0.11625 };
                case 98:
                    return new LMS { L = -1.345, M = 16.054, S = 0.11679 };
                case 99:
                    return new LMS { L = -1.346, M = 16.085, S = 0.1173 };
                case 100:
                    return new LMS { L = -1.347, M = 16.118, S = 0.1178 };
                case 101:
                    return new LMS { L = -1.347, M = 16.15, S = 0.1183 };
                case 102:
                    return new LMS { L = -1.348, M = 16.184, S = 0.11879 };
                case 103:
                    return new LMS { L = -1.348, M = 16.218, S = 0.11926 };
                case 104:
                    return new LMS { L = -1.348, M = 16.253, S = 0.11972 };
                case 105:
                    return new LMS { L = -1.349, M = 16.288, S = 0.12017 };
                case 106:
                    return new LMS { L = -1.349, M = 16.324, S = 0.1206 };
                case 107:
                    return new LMS { L = -1.348, M = 16.361, S = 0.12103 };
                case 108:
                    return new LMS { L = -1.348, M = 16.399, S = 0.12144 };
                case 109:
                    return new LMS { L = -1.348, M = 16.437, S = 0.12185 };
                case 110:
                    return new LMS { L = -1.347, M = 16.475, S = 0.12223 };
                case 111:
                    return new LMS { L = -1.347, M = 16.515, S = 0.12261 };
                case 112:
                    return new LMS { L = -1.346, M = 16.555, S = 0.12298 };
                case 113:
                    return new LMS { L = -1.346, M = 16.596, S = 0.12333 };
                case 114:
                    return new LMS { L = -1.345, M = 16.637, S = 0.12367 };
                case 115:
                    return new LMS { L = -1.344, M = 16.679, S = 0.124 };
                case 116:
                    return new LMS { L = -1.343, M = 16.722, S = 0.12432 };
                case 117:
                    return new LMS { L = -1.342, M = 16.765, S = 0.12462 };
                case 118:
                    return new LMS { L = -1.341, M = 16.809, S = 0.12492 };
                case 119:
                    return new LMS { L = -1.34, M = 16.853, S = 0.1252 };
                case 120:
                    return new LMS { L = -1.339, M = 16.898, S = 0.12547 };
                case 121:
                    return new LMS { L = -1.338, M = 16.943, S = 0.12573 };
                case 122:
                    return new LMS { L = -1.337, M = 16.99, S = 0.12598 };
                case 123:
                    return new LMS { L = -1.336, M = 17.036, S = 0.12622 };
                case 124:
                    return new LMS { L = -1.334, M = 17.083, S = 0.12644 };
                case 125:
                    return new LMS { L = -1.333, M = 17.131, S = 0.12666 };
                case 126:
                    return new LMS { L = -1.332, M = 17.179, S = 0.12687 };
                case 127:
                    return new LMS { L = -1.33, M = 17.227, S = 0.12706 };
                case 128:
                    return new LMS { L = -1.329, M = 17.277, S = 0.12725 };
                case 129:
                    return new LMS { L = -1.327, M = 17.327, S = 0.12742 };
                case 130:
                    return new LMS { L = -1.326, M = 17.377, S = 0.12759 };
                case 131:
                    return new LMS { L = -1.324, M = 17.427, S = 0.12774 };
                case 132:
                    return new LMS { L = -1.322, M = 17.478, S = 0.12789 };
                case 133:
                    return new LMS { L = -1.321, M = 17.53, S = 0.12803 };
                case 134:
                    return new LMS { L = -1.319, M = 17.581, S = 0.12816 };
                case 135:
                    return new LMS { L = -1.318, M = 17.634, S = 0.12827 };
                case 136:
                    return new LMS { L = -1.316, M = 17.687, S = 0.12838 };
                case 137:
                    return new LMS { L = -1.314, M = 17.739, S = 0.12849 };
                case 138:
                    return new LMS { L = -1.312, M = 17.793, S = 0.12858 };
                case 139:
                    return new LMS { L = -1.311, M = 17.846, S = 0.12866 };
                case 140:
                    return new LMS { L = -1.309, M = 17.9, S = 0.12875 };
                case 141:
                    return new LMS { L = -1.307, M = 17.954, S = 0.12882 };
                case 142:
                    return new LMS { L = -1.306, M = 18.008, S = 0.12888 };
                case 143:
                    return new LMS { L = -1.304, M = 18.062, S = 0.12894 };
                case 144:
                    return new LMS { L = -1.302, M = 18.117, S = 0.12899 };
                case 145:
                    return new LMS { L = -1.3, M = 18.172, S = 0.12903 };
                case 146:
                    return new LMS { L = -1.299, M = 18.227, S = 0.12907 };
                case 147:
                    return new LMS { L = -1.297, M = 18.281, S = 0.1291 };
                case 148:
                    return new LMS { L = -1.295, M = 18.337, S = 0.12913 };
                case 149:
                    return new LMS { L = -1.293, M = 18.391, S = 0.12915 };
                case 150:
                    return new LMS { L = -1.291, M = 18.446, S = 0.12917 };
                case 151:
                    return new LMS { L = -1.29, M = 18.5, S = 0.12918 };
                case 152:
                    return new LMS { L = -1.288, M = 18.555, S = 0.12918 };
                case 153:
                    return new LMS { L = -1.286, M = 18.61, S = 0.12919 };
                case 154:
                    return new LMS { L = -1.284, M = 18.664, S = 0.12919 };
                case 155:
                    return new LMS { L = -1.283, M = 18.718, S = 0.12918 };
                case 156:
                    return new LMS { L = -1.281, M = 18.772, S = 0.12917 };
                case 157:
                    return new LMS { L = -1.279, M = 18.825, S = 0.12916 };
                case 158:
                    return new LMS { L = -1.277, M = 18.88, S = 0.12914 };
                case 159:
                    return new LMS { L = -1.276, M = 18.932, S = 0.12913 };
                case 160:
                    return new LMS { L = -1.274, M = 18.985, S = 0.1291 };
                case 161:
                    return new LMS { L = -1.272, M = 19.038, S = 0.12908 };
                case 162:
                    return new LMS { L = -1.271, M = 19.09, S = 0.12905 };
                case 163:
                    return new LMS { L = -1.269, M = 19.142, S = 0.12902 };
                case 164:
                    return new LMS { L = -1.267, M = 19.194, S = 0.12899 };
                case 165:
                    return new LMS { L = -1.266, M = 19.244, S = 0.12895 };
                case 166:
                    return new LMS { L = -1.264, M = 19.295, S = 0.12892 };
                case 167:
                    return new LMS { L = -1.262, M = 19.345, S = 0.12888 };
                case 168:
                    return new LMS { L = -1.261, M = 19.395, S = 0.12884 };
                case 169:
                    return new LMS { L = -1.259, M = 19.445, S = 0.12879 };
                case 170:
                    return new LMS { L = -1.258, M = 19.493, S = 0.12875 };
                case 171:
                    return new LMS { L = -1.256, M = 19.542, S = 0.1287 };
                case 172:
                    return new LMS { L = -1.254, M = 19.589, S = 0.12866 };
                case 173:
                    return new LMS { L = -1.253, M = 19.637, S = 0.12861 };
                case 174:
                    return new LMS { L = -1.251, M = 19.684, S = 0.12856 };
                case 175:
                    return new LMS { L = -1.25, M = 19.73, S = 0.12851 };
                case 176:
                    return new LMS { L = -1.248, M = 19.776, S = 0.12846 };
                case 177:
                    return new LMS { L = -1.247, M = 19.822, S = 0.1284 };
                case 178:
                    return new LMS { L = -1.245, M = 19.866, S = 0.12835 };
                case 179:
                    return new LMS { L = -1.244, M = 19.911, S = 0.1283 };
                case 180:
                    return new LMS { L = -1.242, M = 19.955, S = 0.12824 };
                case 181:
                    return new LMS { L = -1.241, M = 19.998, S = 0.12819 };
                case 182:
                    return new LMS { L = -1.239, M = 20.041, S = 0.12813 };
                case 183:
                    return new LMS { L = -1.238, M = 20.083, S = 0.12807 };
                case 184:
                    return new LMS { L = -1.236, M = 20.124, S = 0.12802 };
                case 185:
                    return new LMS { L = -1.235, M = 20.166, S = 0.12796 };
                case 186:
                    return new LMS { L = -1.233, M = 20.206, S = 0.1279 };
                case 187:
                    return new LMS { L = -1.232, M = 20.246, S = 0.12785 };
                case 188:
                    return new LMS { L = -1.231, M = 20.285, S = 0.12779 };
                case 189:
                    return new LMS { L = -1.229, M = 20.324, S = 0.12773 };
                case 190:
                    return new LMS { L = -1.228, M = 20.363, S = 0.12768 };
                case 191:
                    return new LMS { L = -1.226, M = 20.401, S = 0.12762 };
                case 192:
                    return new LMS { L = -1.225, M = 20.438, S = 0.12757 };
                case 193:
                    return new LMS { L = -1.224, M = 20.475, S = 0.12751 };
                case 194:
                    return new LMS { L = -1.222, M = 20.511, S = 0.12745 };
                case 195:
                    return new LMS { L = -1.221, M = 20.547, S = 0.1274 };
                case 196:
                    return new LMS { L = -1.22, M = 20.582, S = 0.12734 };
                case 197:
                    return new LMS { L = -1.218, M = 20.617, S = 0.12729 };
                case 198:
                    return new LMS { L = -1.217, M = 20.652, S = 0.12723 };
                case 199:
                    return new LMS { L = -1.216, M = 20.685, S = 0.12718 };
                case 200:
                    return new LMS { L = -1.214, M = 20.718, S = 0.12712 };
                case 201:
                    return new LMS { L = -1.213, M = 20.751, S = 0.12707 };
                case 202:
                    return new LMS { L = -1.212, M = 20.783, S = 0.12702 };
                case 203:
                    return new LMS { L = -1.21, M = 20.816, S = 0.12696 };
                case 204:
                    return new LMS { L = -1.209, M = 20.847, S = 0.12691 };
                case 205:
                    return new LMS { L = -1.208, M = 20.878, S = 0.12686 };
                case 206:
                    return new LMS { L = -1.206, M = 20.908, S = 0.12681 };
                case 207:
                    return new LMS { L = -1.205, M = 20.938, S = 0.12676 };
                case 208:
                    return new LMS { L = -1.204, M = 20.968, S = 0.12671 };
                case 209:
                    return new LMS { L = -1.203, M = 20.997, S = 0.12666 };
                case 210:
                    return new LMS { L = -1.201, M = 21.026, S = 0.1266 };
                case 211:
                    return new LMS { L = -1.2, M = 21.054, S = 0.12656 };
                case 212:
                    return new LMS { L = -1.199, M = 21.082, S = 0.1265 };
                case 213:
                    return new LMS { L = -1.197, M = 21.11, S = 0.12646 };
                case 214:
                    return new LMS { L = -1.196, M = 21.137, S = 0.12641 };
                case 215:
                    return new LMS { L = -1.195, M = 21.164, S = 0.12636 };
                case 216:
                    return new LMS { L = -1.194, M = 21.19, S = 0.12631 };
                case 217:
                    return new LMS { L = -1.193, M = 21.216, S = 0.12627 };
                case 218:
                    return new LMS { L = -1.191, M = 21.242, S = 0.12622 };
                case 219:
                    return new LMS { L = -1.19, M = 21.267, S = 0.12617 };
                case 220:
                    return new LMS { L = -1.189, M = 21.293, S = 0.12613 };
                case 221:
                    return new LMS { L = -1.188, M = 21.317, S = 0.12608 };
                case 222:
                    return new LMS { L = -1.186, M = 21.342, S = 0.12604 };
                case 223:
                    return new LMS { L = -1.185, M = 21.366, S = 0.126 };
                case 224:
                    return new LMS { L = -1.184, M = 21.39, S = 0.12595 };
                case 225:
                    return new LMS { L = -1.183, M = 21.413, S = 0.12591 };
                case 226:
                    return new LMS { L = -1.181, M = 21.436, S = 0.12587 };
                case 227:
                    return new LMS { L = -1.18, M = 21.459, S = 0.12582 };
                case 228:
                    return new LMS { L = -1.179, M = 21.482, S = 0.12578 };
                case 229:
                    return new LMS { L = -1.178, M = 21.504, S = 0.12574 };
                case 230:
                    return new LMS { L = -1.177, M = 21.527, S = 0.1257 };
                case 231:
                    return new LMS { L = -1.175, M = 21.548, S = 0.12566 };
                case 232:
                    return new LMS { L = -1.174, M = 21.57, S = 0.12561 };
                case 233:
                    return new LMS { L = -1.173, M = 21.591, S = 0.12558 };
                case 234:
                    return new LMS { L = -1.172, M = 21.612, S = 0.12554 };
                case 235:
                    return new LMS { L = -1.171, M = 21.633, S = 0.1255 };
                case 236:
                    return new LMS { L = -1.169, M = 21.653, S = 0.12546 };
                case 237:
                    return new LMS { L = -1.168, M = 21.674, S = 0.12542 };
                case 238:
                    return new LMS { L = -1.167, M = 21.695, S = 0.12538 };
                case 239:
                    return new LMS { L = -1.166, M = 21.715, S = 0.12534 };
                case 240:
                    return new LMS { L = -1.165, M = 21.735, S = 0.1253 };
                default:
                    throw new ArgumentOutOfRangeException("ageMonths");
            }
        }
    }
}