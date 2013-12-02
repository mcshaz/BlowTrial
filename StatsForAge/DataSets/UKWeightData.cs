using System;
namespace StatsForAge.DataSets
{
    /// <summary>
    /// centile data for weight in kg
    /// </summary>
    public sealed class UKWeightData : CentileData
    {
        protected override LMS LMSForGestAge(int gestAgeWeeks, bool isMale)
        {
            if (isMale)
            {
                switch (gestAgeWeeks)
                {
                    case 23:
                        return new LMS { L = 1.147, M = 0.6145, S = 0.15875 };
                    case 24:
                        return new LMS { L = 1.126, M = 0.7142, S = 0.16249 };
                    case 25:
                        return new LMS { L = 1.104, M = 0.8167, S = 0.16628 };
                    case 26:
                        return new LMS { L = 1.083, M = 0.9244, S = 0.17007 };
                    case 27:
                        return new LMS { L = 1.061, M = 1.0364, S = 0.17355 };
                    case 28:
                        return new LMS { L = 1.04, M = 1.1577, S = 0.17663 };
                    case 29:
                        return new LMS { L = 1.018, M = 1.2898, S = 0.17905 };
                    case 30:
                        return new LMS { L = 0.997, M = 1.436, S = 0.18056 };
                    case 31:
                        return new LMS { L = 0.975, M = 1.605, S = 0.18092 };
                    case 32:
                        return new LMS { L = 0.954, M = 1.7993, S = 0.1798 };
                    case 33:
                        return new LMS { L = 0.932, M = 2.0156, S = 0.17703 };
                    case 34:
                        return new LMS { L = 0.911, M = 2.2472, S = 0.17262 };
                    case 35:
                        return new LMS { L = 0.889, M = 2.486, S = 0.16668 };
                    case 36:
                        return new LMS { L = 0.868, M = 2.7257, S = 0.15938 };
                    case 37:
                        return new LMS { L = 0.846, M = 2.9594, S = 0.15117 };
                    case 38:
                        return new LMS { L = 0.825, M = 3.1778, S = 0.14258 };
                    case 39:
                        return new LMS { L = 0.803, M = 3.3769, S = 0.13469 };
                    case 40:
                        return new LMS { L = 0.782, M = 3.5551, S = 0.12851 };
                    case 41:
                        return new LMS { L = 0.76, M = 3.7172, S = 0.12412 };
                    case 42:
                        return new LMS { L = 0.739, M = 3.8702, S = 0.12085 };
                    case 43:
                        return new LMS { L = 244.2, M = 4.0603, S = 138.07 };
                    default:
                        throw new ArgumentOutOfRangeException("gestAgeWeeks");
                }
            }
            switch (gestAgeWeeks) //Female
            {
                case 23:
                    return new LMS { L = 1.326, M = 0.5589, S = 0.17378 };
                case 24:
                    return new LMS { L = 1.278, M = 0.6584, S = 0.17716 };
                case 25:
                    return new LMS { L = 1.229, M = 0.7611, S = 0.18066 };
                case 26:
                    return new LMS { L = 1.181, M = 0.8672, S = 0.18429 };
                case 27:
                    return new LMS { L = 1.132, M = 0.9775, S = 0.18779 };
                case 28:
                    return new LMS { L = 1.084, M = 1.0929, S = 0.19058 };
                case 29:
                    return new LMS { L = 1.035, M = 1.2166, S = 0.19209 };
                case 30:
                    return new LMS { L = 0.987, M = 1.3593, S = 0.19212 };
                case 31:
                    return new LMS { L = 0.938, M = 1.525, S = 0.19052 };
                case 32:
                    return new LMS { L = 0.89, M = 1.7118, S = 0.1873 };
                case 33:
                    return new LMS { L = 0.841, M = 1.9163, S = 0.18261 };
                case 34:
                    return new LMS { L = 0.793, M = 2.1342, S = 0.17659 };
                case 35:
                    return new LMS { L = 0.744, M = 2.3607, S = 0.1694 };
                case 36:
                    return new LMS { L = 0.695, M = 2.5903, S = 0.16107 };
                case 37:
                    return new LMS { L = 0.647, M = 2.8164, S = 0.15165 };
                case 38:
                    return new LMS { L = 0.598, M = 3.0334, S = 0.14174 };
                case 39:
                    return new LMS { L = 0.55, M = 3.2362, S = 0.13249 };
                case 40:
                    return new LMS { L = 0.501, M = 3.413, S = 0.12481 };
                case 41:
                    return new LMS { L = 0.453, M = 3.5539, S = 0.11855 };
                case 42:
                    return new LMS { L = 0.404, M = 3.6743, S = 0.11308 };
                case 43:
                    return new LMS { L = 0.2024, M = 3.8352, S = 0.1406 };
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
                        return new LMS { L = 233.1, M = 4.3671, S = 134.97 };
                    case 5:
                        return new LMS { L = 223.7, M = 4.659, S = 132.15 };
                    case 6:
                        return new LMS { L = 215.5, M = 4.9303, S = 129.6 };
                    case 7:
                        return new LMS { L = 208.1, M = 5.1817, S = 127.29 };
                    case 8:
                        return new LMS { L = 201.4, M = 5.4149, S = 125.2 };
                    case 9:
                        return new LMS { L = 195.2, M = 5.6319, S = 123.3 };
                    case 10:
                        return new LMS { L = 189.4, M = 5.8346, S = 121.57 };
                    case 11:
                        return new LMS { L = 184, M = 6.0242, S = 120.01 };
                    case 12:
                        return new LMS { L = 178.9, M = 6.2019, S = 118.6 };
                    case 13:
                        return new LMS { L = 174, M = 6.369, S = 117.32 };
                    default:
                        throw new ArgumentOutOfRangeException("ageWeeks");
                }
            }
            switch (ageWeeks) //Female
            {
                case 4:
                    return new LMS { L = 0.1789, M = 4.0987, S = 0.13805 };
                case 5:
                    return new LMS { L = 0.1582, M = 4.3476, S = 0.13583 };
                case 6:
                    return new LMS { L = 0.1395, M = 4.5793, S = 0.13392 };
                case 7:
                    return new LMS { L = 0.1224, M = 4.795, S = 0.13228 };
                case 8:
                    return new LMS { L = 0.1065, M = 4.9959, S = 0.13087 };
                case 9:
                    return new LMS { L = 0.0918, M = 5.1842, S = 0.12966 };
                case 10:
                    return new LMS { L = 0.0779, M = 5.3618, S = 0.12861 };
                case 11:
                    return new LMS { L = 0.0648, M = 5.5295, S = 0.1277 };
                case 12:
                    return new LMS { L = 0.0525, M = 5.6883, S = 0.12691 };
                case 13:
                    return new LMS { L = 0.0407, M = 5.8393, S = 0.12622 };
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
                        return new LMS { L = 0.1738, M = 6.3762, S = 0.11727 };
                    case 4:
                        return new LMS { L = 0.1553, M = 7.0023, S = 0.11316 };
                    case 5:
                        return new LMS { L = 0.1395, M = 7.5105, S = 0.1108 };
                    case 6:
                        return new LMS { L = 0.1257, M = 7.934, S = 0.10958 };
                    case 7:
                        return new LMS { L = 0.1134, M = 8.297, S = 0.10902 };
                    case 8:
                        return new LMS { L = 0.1021, M = 8.6151, S = 0.10882 };
                    case 9:
                        return new LMS { L = 0.0917, M = 8.9014, S = 0.10881 };
                    case 10:
                        return new LMS { L = 0.082, M = 9.1649, S = 0.10891 };
                    case 11:
                        return new LMS { L = 0.073, M = 9.4122, S = 0.10906 };
                    case 12:
                        return new LMS { L = 0.0644, M = 9.6479, S = 0.10925 };
                    case 13:
                        return new LMS { L = 0.0563, M = 9.8749, S = 0.10949 };
                    case 14:
                        return new LMS { L = 0.0487, M = 10.0953, S = 0.10976 };
                    case 15:
                        return new LMS { L = 0.0413, M = 10.3108, S = 0.11007 };
                    case 16:
                        return new LMS { L = 0.0343, M = 10.5228, S = 0.11041 };
                    case 17:
                        return new LMS { L = 0.0275, M = 10.7319, S = 0.11079 };
                    case 18:
                        return new LMS { L = 0.0211, M = 10.9385, S = 0.11119 };
                    case 19:
                        return new LMS { L = 0.0148, M = 11.143, S = 0.11164 };
                    case 20:
                        return new LMS { L = 0.0087, M = 11.3462, S = 0.11211 };
                    case 21:
                        return new LMS { L = 0.0029, M = 11.5486, S = 0.11261 };
                    case 22:
                        return new LMS { L = -0.0028, M = 11.7504, S = 0.11314 };
                    case 23:
                        return new LMS { L = -0.0083, M = 11.9514, S = 0.11369 };
                    case 24:
                        return new LMS { L = -0.0137, M = 12.1515, S = 0.11426 };
                    case 25:
                        return new LMS { L = -0.0189, M = 12.3502, S = 0.11485 };
                    case 26:
                        return new LMS { L = -0.024, M = 12.5466, S = 0.11544 };
                    case 27:
                        return new LMS { L = -0.0289, M = 12.7401, S = 0.11604 };
                    case 28:
                        return new LMS { L = -0.0337, M = 12.9303, S = 0.11664 };
                    case 29:
                        return new LMS { L = -0.0385, M = 13.1169, S = 0.11723 };
                    case 30:
                        return new LMS { L = -0.0431, M = 13.3, S = 0.11781 };
                    case 31:
                        return new LMS { L = -0.0476, M = 13.4798, S = 0.11839 };
                    case 32:
                        return new LMS { L = -0.052, M = 13.6567, S = 0.11896 };
                    case 33:
                        return new LMS { L = -0.0564, M = 13.8309, S = 0.11953 };
                    case 34:
                        return new LMS { L = -0.0606, M = 14.0031, S = 0.12008 };
                    case 35:
                        return new LMS { L = -0.0648, M = 14.1736, S = 0.12062 };
                    case 36:
                        return new LMS { L = -0.0689, M = 14.3429, S = 0.12116 };
                    case 37:
                        return new LMS { L = -0.0729, M = 14.5113, S = 0.12168 };
                    case 38:
                        return new LMS { L = -0.0769, M = 14.6791, S = 0.1222 };
                    case 39:
                        return new LMS { L = -0.0808, M = 14.8466, S = 0.12271 };
                    case 40:
                        return new LMS { L = -0.0846, M = 15.014, S = 0.12322 };
                    case 41:
                        return new LMS { L = -0.0883, M = 15.1813, S = 0.12373 };
                    case 42:
                        return new LMS { L = -0.092, M = 15.3486, S = 0.12425 };
                    case 43:
                        return new LMS { L = -0.0957, M = 15.5158, S = 0.12478 };
                    case 44:
                        return new LMS { L = -0.0993, M = 15.6828, S = 0.12531 };
                    case 45:
                        return new LMS { L = -0.1028, M = 15.8497, S = 0.12586 };
                    case 46:
                        return new LMS { L = -0.1063, M = 16.0163, S = 0.12643 };
                    case 47:
                        return new LMS { L = -0.1097, M = 16.1827, S = 0.127 };
                    case 48:
                        return new LMS { L = -0.1131, M = 16.3489, S = 0.12759 };
                    case 49:
                        return new LMS { L = -0.426, M = 16.551, S = 0.11649 };
                    case 50:
                        return new LMS { L = -0.448, M = 16.8689, S = 0.11717 };
                    case 51:
                        return new LMS { L = -0.459, M = 17.033, S = 0.11752 };
                    case 52:
                        return new LMS { L = -0.471, M = 17.2011, S = 0.11788 };
                    case 53:
                        return new LMS { L = -0.482, M = 17.3729, S = 0.11826 };
                    case 54:
                        return new LMS { L = -0.494, M = 17.548, S = 0.11864 };
                    case 55:
                        return new LMS { L = -0.506, M = 17.7253, S = 0.11903 };
                    case 56:
                        return new LMS { L = -0.518, M = 17.9043, S = 0.11942 };
                    case 57:
                        return new LMS { L = -0.53, M = 18.086, S = 0.11983 };
                    case 58:
                        return new LMS { L = -0.543, M = 18.2673, S = 0.12025 };
                    case 59:
                        return new LMS { L = -0.555, M = 18.4507, S = 0.12067 };
                    case 60:
                        return new LMS { L = -0.567, M = 18.633, S = 0.1211 };
                    case 61:
                        return new LMS { L = -0.579, M = 18.8143, S = 0.12154 };
                    case 62:
                        return new LMS { L = -0.591, M = 18.9942, S = 0.12198 };
                    case 63:
                        return new LMS { L = -0.603, M = 19.172, S = 0.12243 };
                    case 64:
                        return new LMS { L = -0.615, M = 19.349, S = 0.12289 };
                    case 65:
                        return new LMS { L = -0.627, M = 19.5253, S = 0.12335 };
                    case 66:
                        return new LMS { L = -0.639, M = 19.701, S = 0.12383 };
                    case 67:
                        return new LMS { L = -0.651, M = 19.8767, S = 0.12432 };
                    case 68:
                        return new LMS { L = -0.663, M = 20.0523, S = 0.12482 };
                    case 69:
                        return new LMS { L = -0.675, M = 20.229, S = 0.12534 };
                    case 70:
                        return new LMS { L = -0.687, M = 20.4077, S = 0.12587 };
                    case 71:
                        return new LMS { L = -0.699, M = 20.5867, S = 0.12642 };
                    case 72:
                        return new LMS { L = -0.711, M = 20.767, S = 0.12699 };
                    case 73:
                        return new LMS { L = -0.723, M = 20.9487, S = 0.12757 };
                    case 74:
                        return new LMS { L = -0.735, M = 21.1303, S = 0.12816 };
                    case 75:
                        return new LMS { L = -0.748, M = 21.313, S = 0.12877 };
                    case 76:
                        return new LMS { L = -0.76, M = 21.4981, S = 0.12939 };
                    case 77:
                        return new LMS { L = -0.772, M = 21.6842, S = 0.13003 };
                    case 78:
                        return new LMS { L = -0.785, M = 21.872, S = 0.13068 };
                    case 79:
                        return new LMS { L = -0.797, M = 22.0621, S = 0.13136 };
                    case 80:
                        return new LMS { L = -0.81, M = 22.2549, S = 0.13205 };
                    case 81:
                        return new LMS { L = -0.823, M = 22.45, S = 0.13276 };
                    case 82:
                        return new LMS { L = -0.836, M = 22.6478, S = 0.13349 };
                    case 83:
                        return new LMS { L = -0.849, M = 22.848, S = 0.13425 };
                    case 84:
                        return new LMS { L = -0.861, M = 23.051, S = 0.13502 };
                    case 85:
                        return new LMS { L = -0.874, M = 23.2576, S = 0.13581 };
                    case 86:
                        return new LMS { L = -0.887, M = 23.4657, S = 0.13662 };
                    case 87:
                        return new LMS { L = -0.9, M = 23.676, S = 0.13744 };
                    case 88:
                        return new LMS { L = -0.913, M = 23.8873, S = 0.13827 };
                    case 89:
                        return new LMS { L = -0.925, M = 24.1001, S = 0.13912 };
                    case 90:
                        return new LMS { L = -0.937, M = 24.314, S = 0.13998 };
                    case 91:
                        return new LMS { L = -0.949, M = 24.5293, S = 0.14084 };
                    case 92:
                        return new LMS { L = -0.961, M = 24.7467, S = 0.14171 };
                    case 93:
                        return new LMS { L = -0.972, M = 24.965, S = 0.14259 };
                    case 94:
                        return new LMS { L = -0.983, M = 25.1849, S = 0.14346 };
                    case 95:
                        return new LMS { L = -0.994, M = 25.407, S = 0.14434 };
                    case 96:
                        return new LMS { L = -1.005, M = 25.63, S = 0.14522 };
                    case 97:
                        return new LMS { L = -1.015, M = 25.855, S = 0.14609 };
                    case 98:
                        return new LMS { L = -1.024, M = 26.0814, S = 0.14696 };
                    case 99:
                        return new LMS { L = -1.034, M = 26.308, S = 0.14782 };
                    case 100:
                        return new LMS { L = -1.042, M = 26.5363, S = 0.14868 };
                    case 101:
                        return new LMS { L = -1.051, M = 26.7655, S = 0.14953 };
                    case 102:
                        return new LMS { L = -1.058, M = 26.996, S = 0.15038 };
                    case 103:
                        return new LMS { L = -1.065, M = 27.2269, S = 0.15121 };
                    case 104:
                        return new LMS { L = -1.072, M = 27.458, S = 0.15204 };
                    case 105:
                        return new LMS { L = -1.078, M = 27.69, S = 0.15286 };
                    case 106:
                        return new LMS { L = -1.083, M = 27.922, S = 0.15366 };
                    case 107:
                        return new LMS { L = -1.088, M = 28.1557, S = 0.15446 };
                    case 108:
                        return new LMS { L = -1.092, M = 28.39, S = 0.15525 };
                    case 109:
                        return new LMS { L = -1.095, M = 28.6266, S = 0.15602 };
                    case 110:
                        return new LMS { L = -1.098, M = 28.8637, S = 0.15677 };
                    case 111:
                        return new LMS { L = -1.101, M = 29.103, S = 0.15751 };
                    case 112:
                        return new LMS { L = -1.103, M = 29.343, S = 0.15824 };
                    case 113:
                        return new LMS { L = -1.104, M = 29.585, S = 0.15895 };
                    case 114:
                        return new LMS { L = -1.105, M = 29.828, S = 0.15963 };
                    case 115:
                        return new LMS { L = -1.105, M = 30.074, S = 0.16031 };
                    case 116:
                        return new LMS { L = -1.104, M = 30.323, S = 0.16096 };
                    case 117:
                        return new LMS { L = -1.103, M = 30.574, S = 0.16161 };
                    case 118:
                        return new LMS { L = -1.102, M = 30.829, S = 0.16224 };
                    case 119:
                        return new LMS { L = -1.099, M = 31.0876, S = 0.16286 };
                    case 120:
                        return new LMS { L = -1.097, M = 31.35, S = 0.16347 };
                    case 121:
                        return new LMS { L = -1.093, M = 31.615, S = 0.16408 };
                    case 122:
                        return new LMS { L = -1.089, M = 31.883, S = 0.16469 };
                    case 123:
                        return new LMS { L = -1.085, M = 32.152, S = 0.16528 };
                    case 124:
                        return new LMS { L = -1.08, M = 32.4224, S = 0.16588 };
                    case 125:
                        return new LMS { L = -1.074, M = 32.6934, S = 0.16648 };
                    case 126:
                        return new LMS { L = -1.068, M = 32.964, S = 0.16707 };
                    case 127:
                        return new LMS { L = -1.061, M = 33.234, S = 0.16766 };
                    case 128:
                        return new LMS { L = -1.053, M = 33.5036, S = 0.16825 };
                    case 129:
                        return new LMS { L = -1.045, M = 33.774, S = 0.16884 };
                    case 130:
                        return new LMS { L = -1.037, M = 34.0437, S = 0.16943 };
                    case 131:
                        return new LMS { L = -1.027, M = 34.3143, S = 0.17001 };
                    case 132:
                        return new LMS { L = -1.018, M = 34.585, S = 0.1706 };
                    case 133:
                        return new LMS { L = -1.007, M = 34.8568, S = 0.17119 };
                    case 134:
                        return new LMS { L = -0.996, M = 35.1298, S = 0.17176 };
                    case 135:
                        return new LMS { L = -0.985, M = 35.405, S = 0.17233 };
                    case 136:
                        return new LMS { L = -0.973, M = 35.6823, S = 0.17289 };
                    case 137:
                        return new LMS { L = -0.96, M = 35.9633, S = 0.17343 };
                    case 138:
                        return new LMS { L = -0.947, M = 36.25, S = 0.17397 };
                    case 139:
                        return new LMS { L = -0.933, M = 36.5432, S = 0.17449 };
                    case 140:
                        return new LMS { L = -0.918, M = 36.8423, S = 0.175 };
                    case 141:
                        return new LMS { L = -0.903, M = 37.149, S = 0.1755 };
                    case 142:
                        return new LMS { L = -0.887, M = 37.4638, S = 0.17599 };
                    case 143:
                        return new LMS { L = -0.87, M = 37.7877, S = 0.17647 };
                    case 144:
                        return new LMS { L = -0.852, M = 38.122, S = 0.17692 };
                    case 145:
                        return new LMS { L = -0.834, M = 38.4653, S = 0.17736 };
                    case 146:
                        return new LMS { L = -0.816, M = 38.8201, S = 0.17778 };
                    case 147:
                        return new LMS { L = -0.796, M = 39.185, S = 0.1782 };
                    case 148:
                        return new LMS { L = -0.776, M = 39.5619, S = 0.1786 };
                    case 149:
                        return new LMS { L = -0.755, M = 39.9494, S = 0.179 };
                    case 150:
                        return new LMS { L = -0.734, M = 40.348, S = 0.17938 };
                    case 151:
                        return new LMS { L = -0.712, M = 40.7577, S = 0.17978 };
                    case 152:
                        return new LMS { L = -0.69, M = 41.1778, S = 0.18017 };
                    case 153:
                        return new LMS { L = -0.668, M = 41.609, S = 0.18057 };
                    case 154:
                        return new LMS { L = -0.645, M = 42.0509, S = 0.18095 };
                    case 155:
                        return new LMS { L = -0.622, M = 42.5052, S = 0.18135 };
                    case 156:
                        return new LMS { L = -0.599, M = 42.97, S = 0.18173 };
                    case 157:
                        return new LMS { L = -0.575, M = 43.4457, S = 0.1821 };
                    case 158:
                        return new LMS { L = -0.552, M = 43.932, S = 0.18245 };
                    case 159:
                        return new LMS { L = -0.53, M = 44.428, S = 0.18278 };
                    case 160:
                        return new LMS { L = -0.507, M = 44.9328, S = 0.18307 };
                    case 161:
                        return new LMS { L = -0.485, M = 45.4445, S = 0.18334 };
                    case 162:
                        return new LMS { L = -0.463, M = 45.963, S = 0.18359 };
                    case 163:
                        return new LMS { L = -0.442, M = 46.4868, S = 0.1838 };
                    case 164:
                        return new LMS { L = -0.422, M = 47.0153, S = 0.18397 };
                    case 165:
                        return new LMS { L = -0.403, M = 47.547, S = 0.18411 };
                    case 166:
                        return new LMS { L = -0.384, M = 48.081, S = 0.18421 };
                    case 167:
                        return new LMS { L = -0.367, M = 48.6173, S = 0.18427 };
                    case 168:
                        return new LMS { L = -0.351, M = 49.154, S = 0.18428 };
                    case 169:
                        return new LMS { L = -0.336, M = 49.6907, S = 0.18425 };
                    case 170:
                        return new LMS { L = -0.322, M = 50.2259, S = 0.18415 };
                    case 171:
                        return new LMS { L = -0.31, M = 50.76, S = 0.18399 };
                    case 172:
                        return new LMS { L = -0.299, M = 51.2915, S = 0.18376 };
                    case 173:
                        return new LMS { L = -0.289, M = 51.8192, S = 0.18346 };
                    case 174:
                        return new LMS { L = -0.281, M = 52.343, S = 0.18309 };
                    case 175:
                        return new LMS { L = -0.275, M = 52.8618, S = 0.18263 };
                    case 176:
                        return new LMS { L = -0.269, M = 53.375, S = 0.18209 };
                    case 177:
                        return new LMS { L = -0.266, M = 53.883, S = 0.18146 };
                    case 178:
                        return new LMS { L = -0.263, M = 54.385, S = 0.18075 };
                    case 179:
                        return new LMS { L = -0.263, M = 54.8807, S = 0.17995 };
                    case 180:
                        return new LMS { L = -0.263, M = 55.369, S = 0.17908 };
                    case 181:
                        return new LMS { L = -0.265, M = 55.851, S = 0.17813 };
                    case 182:
                        return new LMS { L = -0.268, M = 56.3242, S = 0.1771 };
                    case 183:
                        return new LMS { L = -0.272, M = 56.7897, S = 0.176 };
                    case 184:
                        return new LMS { L = -0.278, M = 57.2474, S = 0.17482 };
                    case 185:
                        return new LMS { L = -0.284, M = 57.6966, S = 0.17357 };
                    case 186:
                        return new LMS { L = -0.292, M = 58.138, S = 0.17226 };
                    case 187:
                        return new LMS { L = -0.3, M = 58.5697, S = 0.17087 };
                    case 188:
                        return new LMS { L = -0.309, M = 58.9933, S = 0.16944 };
                    case 189:
                        return new LMS { L = -0.319, M = 59.4074, S = 0.16797 };
                    case 190:
                        return new LMS { L = -0.33, M = 59.8116, S = 0.16645 };
                    case 191:
                        return new LMS { L = -0.341, M = 60.2047, S = 0.16491 };
                    case 192:
                        return new LMS { L = -0.352, M = 60.588, S = 0.16336 };
                    case 193:
                        return new LMS { L = -0.364, M = 60.9597, S = 0.1618 };
                    case 194:
                        return new LMS { L = -0.376, M = 61.3195, S = 0.16025 };
                    case 195:
                        return new LMS { L = -0.387, M = 61.668, S = 0.15872 };
                    case 196:
                        return new LMS { L = -0.399, M = 62.0053, S = 0.15722 };
                    case 197:
                        return new LMS { L = -0.411, M = 62.3324, S = 0.15576 };
                    case 198:
                        return new LMS { L = -0.422, M = 62.647, S = 0.15434 };
                    case 199:
                        return new LMS { L = -0.434, M = 62.9519, S = 0.15297 };
                    case 200:
                        return new LMS { L = -0.445, M = 63.2463, S = 0.15166 };
                    case 201:
                        return new LMS { L = -0.456, M = 63.531, S = 0.15041 };
                    case 202:
                        return new LMS { L = -0.467, M = 63.806, S = 0.14922 };
                    case 203:
                        return new LMS { L = -0.478, M = 64.0717, S = 0.14807 };
                    case 204:
                        return new LMS { L = -0.489, M = 64.329, S = 0.14699 };
                    case 205:
                        return new LMS { L = -0.5, M = 64.5782, S = 0.14596 };
                    case 206:
                        return new LMS { L = -0.51, M = 64.8187, S = 0.14498 };
                    case 207:
                        return new LMS { L = -0.52, M = 65.051, S = 0.14407 };
                    case 208:
                        return new LMS { L = -0.53, M = 65.2768, S = 0.1432 };
                    case 209:
                        return new LMS { L = -0.539, M = 65.4933, S = 0.14238 };
                    case 210:
                        return new LMS { L = -0.549, M = 65.704, S = 0.14161 };
                    case 211:
                        return new LMS { L = -0.558, M = 65.9069, S = 0.14088 };
                    case 212:
                        return new LMS { L = -0.567, M = 66.1033, S = 0.1402 };
                    case 213:
                        return new LMS { L = -0.575, M = 66.292, S = 0.13956 };
                    case 214:
                        return new LMS { L = -0.583, M = 66.4765, S = 0.13896 };
                    case 215:
                        return new LMS { L = -0.591, M = 66.6541, S = 0.13839 };
                    case 216:
                        return new LMS { L = -0.599, M = 66.824, S = 0.13786 };
                    case 217:
                        return new LMS { L = -0.606, M = 66.9883, S = 0.13735 };
                    case 218:
                        return new LMS { L = -0.613, M = 67.147, S = 0.13688 };
                    case 219:
                        return new LMS { L = -0.62, M = 67.3, S = 0.13643 };
                    case 220:
                        return new LMS { L = -0.627, M = 67.448, S = 0.13601 };
                    case 221:
                        return new LMS { L = -0.633, M = 67.5898, S = 0.13561 };
                    case 222:
                        return new LMS { L = -0.639, M = 67.728, S = 0.13523 };
                    case 223:
                        return new LMS { L = -0.645, M = 67.8607, S = 0.13488 };
                    case 224:
                        return new LMS { L = -0.651, M = 67.988, S = 0.13453 };
                    case 225:
                        return new LMS { L = -0.656, M = 68.111, S = 0.13422 };
                    case 226:
                        return new LMS { L = -0.662, M = 68.2298, S = 0.13392 };
                    case 227:
                        return new LMS { L = -0.667, M = 68.3437, S = 0.13363 };
                    case 228:
                        return new LMS { L = -0.671, M = 68.454, S = 0.13335 };
                    case 229:
                        return new LMS { L = -0.676, M = 68.5595, S = 0.1331 };
                    case 230:
                        return new LMS { L = -0.68, M = 68.6613, S = 0.13284 };
                    case 231:
                        return new LMS { L = -0.685, M = 68.76, S = 0.13261 };
                    case 232:
                        return new LMS { L = -0.689, M = 68.8555, S = 0.13239 };
                    case 233:
                        return new LMS { L = -0.693, M = 68.9467, S = 0.13217 };
                    case 234:
                        return new LMS { L = -0.697, M = 69.036, S = 0.13197 };
                    case 235:
                        return new LMS { L = -0.7, M = 69.1223, S = 0.13177 };
                    case 236:
                        return new LMS { L = -0.704, M = 69.2067, S = 0.13158 };
                    case 237:
                        return new LMS { L = -0.708, M = 69.289, S = 0.13139 };
                    case 238:
                        return new LMS { L = -0.711, M = 69.3693, S = 0.13122 };
                    case 239:
                        return new LMS { L = -0.714, M = 69.447, S = 0.13105 };
                    case 240:
                        return new LMS { L = -0.718, M = 69.524, S = 0.13088 };
                    default:
                        throw new ArgumentOutOfRangeException("ageMonths");
                }
            }
            switch (ageMonths) //Female
            {
                case 3:
                    return new LMS { L = 0.0402, M = 5.8458, S = 0.12619 };
                case 4:
                    return new LMS { L = -0.005, M = 6.4237, S = 0.12402 };
                case 5:
                    return new LMS { L = -0.043, M = 6.8985, S = 0.12274 };
                case 6:
                    return new LMS { L = -0.0756, M = 7.297, S = 0.12204 };
                case 7:
                    return new LMS { L = -0.1039, M = 7.6422, S = 0.12178 };
                case 8:
                    return new LMS { L = -0.1288, M = 7.9487, S = 0.12181 };
                case 9:
                    return new LMS { L = -0.1507, M = 8.2254, S = 0.12199 };
                case 10:
                    return new LMS { L = -0.17, M = 8.48, S = 0.12223 };
                case 11:
                    return new LMS { L = -0.1872, M = 8.7192, S = 0.12247 };
                case 12:
                    return new LMS { L = -0.2024, M = 8.9481, S = 0.12268 };
                case 13:
                    return new LMS { L = -0.2158, M = 9.1699, S = 0.12283 };
                case 14:
                    return new LMS { L = -0.2278, M = 9.387, S = 0.12294 };
                case 15:
                    return new LMS { L = -0.2384, M = 9.6008, S = 0.12299 };
                case 16:
                    return new LMS { L = -0.2478, M = 9.8124, S = 0.12303 };
                case 17:
                    return new LMS { L = -0.2562, M = 10.0226, S = 0.12306 };
                case 18:
                    return new LMS { L = -0.2637, M = 10.2315, S = 0.12309 };
                case 19:
                    return new LMS { L = -0.2703, M = 10.4393, S = 0.12315 };
                case 20:
                    return new LMS { L = -0.2762, M = 10.6464, S = 0.12323 };
                case 21:
                    return new LMS { L = -0.2815, M = 10.8534, S = 0.12335 };
                case 22:
                    return new LMS { L = -0.2862, M = 11.0608, S = 0.1235 };
                case 23:
                    return new LMS { L = -0.2903, M = 11.2688, S = 0.12369 };
                case 24:
                    return new LMS { L = -0.2941, M = 11.4775, S = 0.1239 };
                case 25:
                    return new LMS { L = -0.2975, M = 11.6864, S = 0.12414 };
                case 26:
                    return new LMS { L = -0.3005, M = 11.8947, S = 0.12441 };
                case 27:
                    return new LMS { L = -0.3032, M = 12.1015, S = 0.12472 };
                case 28:
                    return new LMS { L = -0.3057, M = 12.3059, S = 0.12506 };
                case 29:
                    return new LMS { L = -0.308, M = 12.5073, S = 0.12545 };
                case 30:
                    return new LMS { L = -0.3101, M = 12.7055, S = 0.12587 };
                case 31:
                    return new LMS { L = -0.312, M = 12.9006, S = 0.12633 };
                case 32:
                    return new LMS { L = -0.3138, M = 13.093, S = 0.12683 };
                case 33:
                    return new LMS { L = -0.3155, M = 13.2837, S = 0.12737 };
                case 34:
                    return new LMS { L = -0.3171, M = 13.4731, S = 0.12794 };
                case 35:
                    return new LMS { L = -0.3186, M = 13.6618, S = 0.12855 };
                case 36:
                    return new LMS { L = -0.3201, M = 13.8503, S = 0.12919 };
                case 37:
                    return new LMS { L = -0.3216, M = 14.0385, S = 0.12988 };
                case 38:
                    return new LMS { L = -0.323, M = 14.2265, S = 0.13059 };
                case 39:
                    return new LMS { L = -0.3243, M = 14.414, S = 0.13135 };
                case 40:
                    return new LMS { L = -0.3257, M = 14.601, S = 0.13213 };
                case 41:
                    return new LMS { L = -0.327, M = 14.7873, S = 0.13293 };
                case 42:
                    return new LMS { L = -0.3283, M = 14.9727, S = 0.13376 };
                case 43:
                    return new LMS { L = -0.3296, M = 15.1573, S = 0.1346 };
                case 44:
                    return new LMS { L = -0.3309, M = 15.341, S = 0.13545 };
                case 45:
                    return new LMS { L = -0.3322, M = 15.524, S = 0.1363 };
                case 46:
                    return new LMS { L = -0.3335, M = 15.7064, S = 0.13716 };
                case 47:
                    return new LMS { L = -0.3348, M = 15.8882, S = 0.138 };
                case 48:
                    return new LMS { L = -0.3361, M = 16.0697, S = 0.13884 };
                case 49:
                    return new LMS { L = -0.516, M = 16.198, S = 0.1244 };
                case 50:
                    return new LMS { L = -0.532, M = 16.5359, S = 0.12584 };
                case 51:
                    return new LMS { L = -0.539, M = 16.706, S = 0.12657 };
                case 52:
                    return new LMS { L = -0.547, M = 16.8781, S = 0.12731 };
                case 53:
                    return new LMS { L = -0.555, M = 17.0519, S = 0.12806 };
                case 54:
                    return new LMS { L = -0.563, M = 17.227, S = 0.12881 };
                case 55:
                    return new LMS { L = -0.57, M = 17.4041, S = 0.12957 };
                case 56:
                    return new LMS { L = -0.578, M = 17.5819, S = 0.13033 };
                case 57:
                    return new LMS { L = -0.585, M = 17.761, S = 0.1311 };
                case 58:
                    return new LMS { L = -0.592, M = 17.9401, S = 0.13186 };
                case 59:
                    return new LMS { L = -0.6, M = 18.1198, S = 0.13264 };
                case 60:
                    return new LMS { L = -0.607, M = 18.299, S = 0.1334 };
                case 61:
                    return new LMS { L = -0.614, M = 18.4791, S = 0.13416 };
                case 62:
                    return new LMS { L = -0.621, M = 18.6581, S = 0.13493 };
                case 63:
                    return new LMS { L = -0.627, M = 18.837, S = 0.13569 };
                case 64:
                    return new LMS { L = -0.634, M = 19.017, S = 0.13643 };
                case 65:
                    return new LMS { L = -0.64, M = 19.1967, S = 0.13719 };
                case 66:
                    return new LMS { L = -0.647, M = 19.377, S = 0.13794 };
                case 67:
                    return new LMS { L = -0.653, M = 19.5581, S = 0.13868 };
                case 68:
                    return new LMS { L = -0.659, M = 19.7404, S = 0.13942 };
                case 69:
                    return new LMS { L = -0.665, M = 19.925, S = 0.14016 };
                case 70:
                    return new LMS { L = -0.672, M = 20.11, S = 0.1409 };
                case 71:
                    return new LMS { L = -0.678, M = 20.2972, S = 0.14165 };
                case 72:
                    return new LMS { L = -0.684, M = 20.486, S = 0.14239 };
                case 73:
                    return new LMS { L = -0.689, M = 20.6778, S = 0.14315 };
                case 74:
                    return new LMS { L = -0.695, M = 20.871, S = 0.14391 };
                case 75:
                    return new LMS { L = -0.701, M = 21.066, S = 0.14468 };
                case 76:
                    return new LMS { L = -0.707, M = 21.2629, S = 0.14547 };
                case 77:
                    return new LMS { L = -0.712, M = 21.4633, S = 0.14626 };
                case 78:
                    return new LMS { L = -0.718, M = 21.667, S = 0.14706 };
                case 79:
                    return new LMS { L = -0.723, M = 21.8742, S = 0.14788 };
                case 80:
                    return new LMS { L = -0.729, M = 22.086, S = 0.14872 };
                case 81:
                    return new LMS { L = -0.734, M = 22.302, S = 0.14958 };
                case 82:
                    return new LMS { L = -0.739, M = 22.5234, S = 0.15045 };
                case 83:
                    return new LMS { L = -0.744, M = 22.749, S = 0.15134 };
                case 84:
                    return new LMS { L = -0.749, M = 22.98, S = 0.15225 };
                case 85:
                    return new LMS { L = -0.754, M = 23.2141, S = 0.15317 };
                case 86:
                    return new LMS { L = -0.758, M = 23.4517, S = 0.15409 };
                case 87:
                    return new LMS { L = -0.762, M = 23.691, S = 0.155 };
                case 88:
                    return new LMS { L = -0.766, M = 23.933, S = 0.15591 };
                case 89:
                    return new LMS { L = -0.77, M = 24.1756, S = 0.15682 };
                case 90:
                    return new LMS { L = -0.774, M = 24.419, S = 0.15772 };
                case 91:
                    return new LMS { L = -0.777, M = 24.6625, S = 0.1586 };
                case 92:
                    return new LMS { L = -0.78, M = 24.907, S = 0.15947 };
                case 93:
                    return new LMS { L = -0.783, M = 25.153, S = 0.16033 };
                case 94:
                    return new LMS { L = -0.785, M = 25.398, S = 0.16117 };
                case 95:
                    return new LMS { L = -0.787, M = 25.6434, S = 0.16201 };
                case 96:
                    return new LMS { L = -0.789, M = 25.889, S = 0.16282 };
                case 97:
                    return new LMS { L = -0.79, M = 26.1335, S = 0.16363 };
                case 98:
                    return new LMS { L = -0.792, M = 26.378, S = 0.16443 };
                case 99:
                    return new LMS { L = -0.792, M = 26.622, S = 0.16521 };
                case 100:
                    return new LMS { L = -0.793, M = 26.8659, S = 0.16599 };
                case 101:
                    return new LMS { L = -0.793, M = 27.1105, S = 0.16677 };
                case 102:
                    return new LMS { L = -0.793, M = 27.355, S = 0.16754 };
                case 103:
                    return new LMS { L = -0.793, M = 27.6011, S = 0.16832 };
                case 104:
                    return new LMS { L = -0.792, M = 27.85, S = 0.16909 };
                case 105:
                    return new LMS { L = -0.791, M = 28.101, S = 0.16987 };
                case 106:
                    return new LMS { L = -0.789, M = 28.3567, S = 0.17064 };
                case 107:
                    return new LMS { L = -0.787, M = 28.6159, S = 0.17141 };
                case 108:
                    return new LMS { L = -0.785, M = 28.88, S = 0.17218 };
                case 109:
                    return new LMS { L = -0.782, M = 29.1481, S = 0.17294 };
                case 110:
                    return new LMS { L = -0.779, M = 29.4203, S = 0.17369 };
                case 111:
                    return new LMS { L = -0.776, M = 29.697, S = 0.17445 };
                case 112:
                    return new LMS { L = -0.772, M = 29.9771, S = 0.1752 };
                case 113:
                    return new LMS { L = -0.768, M = 30.2597, S = 0.17594 };
                case 114:
                    return new LMS { L = -0.764, M = 30.545, S = 0.17668 };
                case 115:
                    return new LMS { L = -0.758, M = 30.8335, S = 0.17742 };
                case 116:
                    return new LMS { L = -0.753, M = 31.1243, S = 0.17816 };
                case 117:
                    return new LMS { L = -0.747, M = 31.417, S = 0.17891 };
                case 118:
                    return new LMS { L = -0.74, M = 31.7122, S = 0.17967 };
                case 119:
                    return new LMS { L = -0.733, M = 32.0096, S = 0.18044 };
                case 120:
                    return new LMS { L = -0.725, M = 32.308, S = 0.18122 };
                case 121:
                    return new LMS { L = -0.717, M = 32.6075, S = 0.18201 };
                case 122:
                    return new LMS { L = -0.708, M = 32.9083, S = 0.1828 };
                case 123:
                    return new LMS { L = -0.699, M = 33.212, S = 0.18359 };
                case 124:
                    return new LMS { L = -0.69, M = 33.5147, S = 0.18437 };
                case 125:
                    return new LMS { L = -0.68, M = 33.8213, S = 0.18514 };
                case 126:
                    return new LMS { L = -0.669, M = 34.129, S = 0.18588 };
                case 127:
                    return new LMS { L = -0.658, M = 34.4393, S = 0.1866 };
                case 128:
                    return new LMS { L = -0.647, M = 34.7518, S = 0.18729 };
                case 129:
                    return new LMS { L = -0.635, M = 35.068, S = 0.18794 };
                case 130:
                    return new LMS { L = -0.623, M = 35.3845, S = 0.18855 };
                case 131:
                    return new LMS { L = -0.61, M = 35.7048, S = 0.18911 };
                case 132:
                    return new LMS { L = -0.597, M = 36.03, S = 0.18961 };
                case 133:
                    return new LMS { L = -0.584, M = 36.3582, S = 0.19004 };
                case 134:
                    return new LMS { L = -0.57, M = 36.6911, S = 0.1904 };
                case 135:
                    return new LMS { L = -0.556, M = 37.029, S = 0.19066 };
                case 136:
                    return new LMS { L = -0.541, M = 37.3731, S = 0.19082 };
                case 137:
                    return new LMS { L = -0.526, M = 37.7197, S = 0.19089 };
                case 138:
                    return new LMS { L = -0.511, M = 38.074, S = 0.19084 };
                case 139:
                    return new LMS { L = -0.496, M = 38.4331, S = 0.19067 };
                case 140:
                    return new LMS { L = -0.48, M = 38.7977, S = 0.19039 };
                case 141:
                    return new LMS { L = -0.465, M = 39.168, S = 0.19 };
                case 142:
                    return new LMS { L = -0.449, M = 39.5443, S = 0.1895 };
                case 143:
                    return new LMS { L = -0.434, M = 39.9272, S = 0.18891 };
                case 144:
                    return new LMS { L = -0.418, M = 40.316, S = 0.18821 };
                case 145:
                    return new LMS { L = -0.403, M = 40.7118, S = 0.18743 };
                case 146:
                    return new LMS { L = -0.388, M = 41.1136, S = 0.18656 };
                case 147:
                    return new LMS { L = -0.374, M = 41.5218, S = 0.18563 };
                case 148:
                    return new LMS { L = -0.36, M = 41.9359, S = 0.18462 };
                case 149:
                    return new LMS { L = -0.347, M = 42.3549, S = 0.18355 };
                case 150:
                    return new LMS { L = -0.335, M = 42.7779, S = 0.18243 };
                case 151:
                    return new LMS { L = -0.324, M = 43.2045, S = 0.18128 };
                case 152:
                    return new LMS { L = -0.313, M = 43.6328, S = 0.18009 };
                case 153:
                    return new LMS { L = -0.304, M = 44.0635, S = 0.17887 };
                case 154:
                    return new LMS { L = -0.295, M = 44.4952, S = 0.17764 };
                case 155:
                    return new LMS { L = -0.288, M = 44.9259, S = 0.17641 };
                case 156:
                    return new LMS { L = -0.281, M = 45.3568, S = 0.17518 };
                case 157:
                    return new LMS { L = -0.276, M = 45.7863, S = 0.17395 };
                case 158:
                    return new LMS { L = -0.271, M = 46.2121, S = 0.17275 };
                case 159:
                    return new LMS { L = -0.268, M = 46.6333, S = 0.17156 };
                case 160:
                    return new LMS { L = -0.266, M = 47.0457, S = 0.1704 };
                case 161:
                    return new LMS { L = -0.265, M = 47.4516, S = 0.16924 };
                case 162:
                    return new LMS { L = -0.265, M = 47.8494, S = 0.16809 };
                case 163:
                    return new LMS { L = -0.266, M = 48.2413, S = 0.16693 };
                case 164:
                    return new LMS { L = -0.267, M = 48.6218, S = 0.16579 };
                case 165:
                    return new LMS { L = -0.27, M = 48.993, S = 0.16465 };
                case 166:
                    return new LMS { L = -0.274, M = 49.3568, S = 0.1635 };
                case 167:
                    return new LMS { L = -0.278, M = 49.7081, S = 0.16237 };
                case 168:
                    return new LMS { L = -0.284, M = 50.052, S = 0.16124 };
                case 169:
                    return new LMS { L = -0.289, M = 50.3846, S = 0.16014 };
                case 170:
                    return new LMS { L = -0.296, M = 50.7117, S = 0.15904 };
                case 171:
                    return new LMS { L = -0.303, M = 51.024, S = 0.15798 };
                case 172:
                    return new LMS { L = -0.31, M = 51.3267, S = 0.15695 };
                case 173:
                    return new LMS { L = -0.318, M = 51.621, S = 0.15595 };
                case 174:
                    return new LMS { L = -0.326, M = 51.903, S = 0.15499 };
                case 175:
                    return new LMS { L = -0.335, M = 52.176, S = 0.15406 };
                case 176:
                    return new LMS { L = -0.343, M = 52.4404, S = 0.15316 };
                case 177:
                    return new LMS { L = -0.352, M = 52.693, S = 0.15232 };
                case 178:
                    return new LMS { L = -0.361, M = 52.9367, S = 0.15152 };
                case 179:
                    return new LMS { L = -0.369, M = 53.1707, S = 0.15075 };
                case 180:
                    return new LMS { L = -0.378, M = 53.3947, S = 0.15005 };
                case 181:
                    return new LMS { L = -0.386, M = 53.6118, S = 0.14938 };
                case 182:
                    return new LMS { L = -0.395, M = 53.8203, S = 0.14875 };
                case 183:
                    return new LMS { L = -0.403, M = 54.022, S = 0.14817 };
                case 184:
                    return new LMS { L = -0.412, M = 54.2157, S = 0.14763 };
                case 185:
                    return new LMS { L = -0.42, M = 54.4033, S = 0.14713 };
                case 186:
                    return new LMS { L = -0.428, M = 54.5842, S = 0.14668 };
                case 187:
                    return new LMS { L = -0.436, M = 54.7587, S = 0.14625 };
                case 188:
                    return new LMS { L = -0.443, M = 54.9271, S = 0.14587 };
                case 189:
                    return new LMS { L = -0.451, M = 55.0889, S = 0.14551 };
                case 190:
                    return new LMS { L = -0.458, M = 55.244, S = 0.14518 };
                case 191:
                    return new LMS { L = -0.465, M = 55.3943, S = 0.14487 };
                case 192:
                    return new LMS { L = -0.472, M = 55.538, S = 0.14459 };
                case 193:
                    return new LMS { L = -0.479, M = 55.6765, S = 0.14433 };
                case 194:
                    return new LMS { L = -0.485, M = 55.8086, S = 0.14409 };
                case 195:
                    return new LMS { L = -0.491, M = 55.9355, S = 0.14387 };
                case 196:
                    return new LMS { L = -0.497, M = 56.0561, S = 0.14366 };
                case 197:
                    return new LMS { L = -0.503, M = 56.1713, S = 0.14347 };
                case 198:
                    return new LMS { L = -0.508, M = 56.2816, S = 0.1433 };
                case 199:
                    return new LMS { L = -0.513, M = 56.3863, S = 0.14314 };
                case 200:
                    return new LMS { L = -0.518, M = 56.4854, S = 0.14299 };
                case 201:
                    return new LMS { L = -0.523, M = 56.581, S = 0.14286 };
                case 202:
                    return new LMS { L = -0.528, M = 56.6713, S = 0.14274 };
                case 203:
                    return new LMS { L = -0.532, M = 56.7567, S = 0.14262 };
                case 204:
                    return new LMS { L = -0.536, M = 56.838, S = 0.14252 };
                case 205:
                    return new LMS { L = -0.54, M = 56.9163, S = 0.14243 };
                case 206:
                    return new LMS { L = -0.544, M = 56.9892, S = 0.14234 };
                case 207:
                    return new LMS { L = -0.547, M = 57.059, S = 0.14226 };
                case 208:
                    return new LMS { L = -0.551, M = 57.1247, S = 0.14218 };
                case 209:
                    return new LMS { L = -0.554, M = 57.1875, S = 0.14211 };
                case 210:
                    return new LMS { L = -0.557, M = 57.2469, S = 0.14204 };
                case 211:
                    return new LMS { L = -0.56, M = 57.3041, S = 0.14198 };
                case 212:
                    return new LMS { L = -0.563, M = 57.357, S = 0.14192 };
                case 213:
                    return new LMS { L = -0.565, M = 57.407, S = 0.14187 };
                case 214:
                    return new LMS { L = -0.568, M = 57.4557, S = 0.14182 };
                case 215:
                    return new LMS { L = -0.57, M = 57.5008, S = 0.14177 };
                case 216:
                    return new LMS { L = -0.572, M = 57.544, S = 0.14173 };
                case 217:
                    return new LMS { L = -0.574, M = 57.5847, S = 0.14168 };
                case 218:
                    return new LMS { L = -0.576, M = 57.6235, S = 0.14165 };
                case 219:
                    return new LMS { L = -0.578, M = 57.66, S = 0.14161 };
                case 220:
                    return new LMS { L = -0.58, M = 57.6953, S = 0.14157 };
                case 221:
                    return new LMS { L = -0.582, M = 57.7288, S = 0.14154 };
                case 222:
                    return new LMS { L = -0.583, M = 57.76, S = 0.14151 };
                case 223:
                    return new LMS { L = -0.585, M = 57.7904, S = 0.14148 };
                case 224:
                    return new LMS { L = -0.586, M = 57.8199, S = 0.14145 };
                case 225:
                    return new LMS { L = -0.588, M = 57.847, S = 0.14142 };
                case 226:
                    return new LMS { L = -0.589, M = 57.8728, S = 0.1414 };
                case 227:
                    return new LMS { L = -0.59, M = 57.897, S = 0.14137 };
                case 228:
                    return new LMS { L = -0.591, M = 57.92, S = 0.14135 };
                case 229:
                    return new LMS { L = -0.593, M = 57.9421, S = 0.14133 };
                case 230:
                    return new LMS { L = -0.594, M = 57.9627, S = 0.14131 };
                case 231:
                    return new LMS { L = -0.595, M = 57.982, S = 0.14129 };
                case 232:
                    return new LMS { L = -0.595, M = 58.0002, S = 0.14127 };
                case 233:
                    return new LMS { L = -0.596, M = 58.0164, S = 0.14126 };
                case 234:
                    return new LMS { L = -0.597, M = 58.032, S = 0.14124 };
                case 235:
                    return new LMS { L = -0.598, M = 58.0464, S = 0.14123 };
                case 236:
                    return new LMS { L = -0.599, M = 58.0597, S = 0.14122 };
                case 237:
                    return new LMS { L = -0.599, M = 58.072, S = 0.1412 };
                case 238:
                    return new LMS { L = -0.6, M = 58.0833, S = 0.14119 };
                case 239:
                    return new LMS { L = -0.6, M = 58.0936, S = 0.14118 };
                case 240:
                    return new LMS { L = -0.601, M = 58.104, S = 0.14117 };
                default:
                    throw new ArgumentOutOfRangeException("ageMonths");
            }
        }
    }
}
