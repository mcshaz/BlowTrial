;window.uKWeightData = function (){
    return new CentileData({
        lMSForGestAge : function(gestAgeWeeks, isMale){
            if (isMale){
                switch (gestAgeWeeks){
                    case 23:
                        return new Lms (  1.147,  0.6145, 0.15875 );
                    case 24:
                        return new Lms (  1.126,  0.7142, 0.16249 );
                    case 25:
                        return new Lms (  1.104,  0.8167, 0.16628 );
                    case 26:
                        return new Lms (  1.083,  0.9244, 0.17007 );
                    case 27:
                        return new Lms (  1.061,  1.0364, 0.17355 );
                    case 28:
                        return new Lms (  1.04,  1.1577, 0.17663 );
                    case 29:
                        return new Lms (  1.018,  1.2898, 0.17905 );
                    case 30:
                        return new Lms (  0.997,  1.436, 0.18056 );
                    case 31:
                        return new Lms (  0.975,  1.605, 0.18092 );
                    case 32:
                        return new Lms (  0.954,  1.7993, 0.1798 );
                    case 33:
                        return new Lms (  0.932,  2.0156, 0.17703 );
                    case 34:
                        return new Lms (  0.911,  2.2472, 0.17262 );
                    case 35:
                        return new Lms (  0.889,  2.486, 0.16668 );
                    case 36:
                        return new Lms (  0.868,  2.7257, 0.15938 );
                    case 37:
                        return new Lms (  0.846,  2.9594, 0.15117 );
                    case 38:
                        return new Lms (  0.825,  3.1778, 0.14258 );
                    case 39:
                        return new Lms (  0.803,  3.3769, 0.13469 );
                    case 40:
                        return new Lms (  0.782,  3.5551, 0.12851 );
                    case 41:
                        return new Lms (  0.76,  3.7172, 0.12412 );
                    case 42:
                        return new Lms (  0.739,  3.8702, 0.12085 );
                    case 43:
                        return new Lms (  244.2,  4.0603, 138.07 );
                    default:
                        throw("gestAgeWeeks");
                }
            }
            switch (gestAgeWeeks) //Female
            {
                case 23:
                    return new Lms (  1.326,  0.5589, 0.17378 );
                case 24:
                    return new Lms (  1.278,  0.6584, 0.17716 );
                case 25:
                    return new Lms (  1.229,  0.7611, 0.18066 );
                case 26:
                    return new Lms (  1.181,  0.8672, 0.18429 );
                case 27:
                    return new Lms (  1.132,  0.9775, 0.18779 );
                case 28:
                    return new Lms (  1.084,  1.0929, 0.19058 );
                case 29:
                    return new Lms (  1.035,  1.2166, 0.19209 );
                case 30:
                    return new Lms (  0.987,  1.3593, 0.19212 );
                case 31:
                    return new Lms (  0.938,  1.525, 0.19052 );
                case 32:
                    return new Lms (  0.89,  1.7118, 0.1873 );
                case 33:
                    return new Lms (  0.841,  1.9163, 0.18261 );
                case 34:
                    return new Lms (  0.793,  2.1342, 0.17659 );
                case 35:
                    return new Lms (  0.744,  2.3607, 0.1694 );
                case 36:
                    return new Lms (  0.695,  2.5903, 0.16107 );
                case 37:
                    return new Lms (  0.647,  2.8164, 0.15165 );
                case 38:
                    return new Lms (  0.598,  3.0334, 0.14174 );
                case 39:
                    return new Lms (  0.55,  3.2362, 0.13249 );
                case 40:
                    return new Lms (  0.501,  3.413, 0.12481 );
                case 41:
                    return new Lms (  0.453,  3.5539, 0.11855 );
                case 42:
                    return new Lms (  0.404,  3.6743, 0.11308 );
                case 43:
                    return new Lms (  0.2024,  3.8352, 0.1406 );
                default:
                    throw ("gestAgeWeeks");
            }
        },
        lMSForAgeWeeks : function(ageWeeks, isMale)
        {
            if (isMale)
            {
                switch (ageWeeks)
                {
                    case 4:
                        return new Lms ( 0.2331,  4.3671, 0.13497 );
                    case 5:
                        return new Lms ( 0.2237,  4.659, 0.13215 );
                    case 6:
                        return new Lms ( 0.2155,  4.9303, 0.1296 );
                    case 7:
                        return new Lms ( 0.2081,  5.1817, 0.12729 );
                    case 8:
                        return new Lms ( 0.2014,  5.4149, 0.1252 );
                    case 9:
                        return new Lms ( 0.1952,  5.6319, 0.1233 );
                    case 10:
                        return new Lms ( 0.1894,  5.8346, 0.12157 );
                    case 11:
                        return new Lms ( 0.184,  6.0242, 0.12001 );
                    case 12:
                        return new Lms ( 0.1789,  6.2019, 0.1186 );
                    case 13:
                        return new Lms ( 0.174,  6.369, 0.11732 );
                    default:
                        throw ("ageWeeks");
                }
            }
            switch (ageWeeks) //Female
            {
                case 4:
                    return new Lms (  0.1789,  4.0987, 0.13805 );
                case 5:
                    return new Lms (  0.1582,  4.3476, 0.13583 );
                case 6:
                    return new Lms (  0.1395,  4.5793, 0.13392 );
                case 7:
                    return new Lms (  0.1224,  4.795, 0.13228 );
                case 8:
                    return new Lms (  0.1065,  4.9959, 0.13087 );
                case 9:
                    return new Lms (  0.0918,  5.1842, 0.12966 );
                case 10:
                    return new Lms (  0.0779,  5.3618, 0.12861 );
                case 11:
                    return new Lms (  0.0648,  5.5295, 0.1277 );
                case 12:
                    return new Lms (  0.0525,  5.6883, 0.12691 );
                case 13:
                    return new Lms (  0.0407,  5.8393, 0.12622 );
                default:
                    throw new ArgumentOutOfRangeException("ageWeeks");
            }
        },
        lMSForAgeMonths : function(ageMonths, isMale)
        {
            if (isMale)
            {
                switch (ageMonths)
                {
                    case 3:
                        return new Lms (  0.1738,  6.3762, 0.11727 );
                    case 4:
                        return new Lms (  0.1553,  7.0023, 0.11316 );
                    case 5:
                        return new Lms (  0.1395,  7.5105, 0.1108 );
                    case 6:
                        return new Lms (  0.1257,  7.934, 0.10958 );
                    case 7:
                        return new Lms (  0.1134,  8.297, 0.10902 );
                    case 8:
                        return new Lms (  0.1021,  8.6151, 0.10882 );
                    case 9:
                        return new Lms (  0.0917,  8.9014, 0.10881 );
                    case 10:
                        return new Lms (  0.082,  9.1649, 0.10891 );
                    case 11:
                        return new Lms (  0.073,  9.4122, 0.10906 );
                    case 12:
                        return new Lms (  0.0644,  9.6479, 0.10925 );
                    case 13:
                        return new Lms (  0.0563,  9.8749, 0.10949 );
                    case 14:
                        return new Lms (  0.0487,  10.0953, 0.10976 );
                    case 15:
                        return new Lms (  0.0413,  10.3108, 0.11007 );
                    case 16:
                        return new Lms (  0.0343,  10.5228, 0.11041 );
                    case 17:
                        return new Lms (  0.0275,  10.7319, 0.11079 );
                    case 18:
                        return new Lms (  0.0211,  10.9385, 0.11119 );
                    case 19:
                        return new Lms (  0.0148,  11.143, 0.11164 );
                    case 20:
                        return new Lms (  0.0087,  11.3462, 0.11211 );
                    case 21:
                        return new Lms (  0.0029,  11.5486, 0.11261 );
                    case 22:
                        return new Lms (  -0.0028,  11.7504, 0.11314 );
                    case 23:
                        return new Lms (  -0.0083,  11.9514, 0.11369 );
                    case 24:
                        return new Lms (  -0.0137,  12.1515, 0.11426 );
                    case 25:
                        return new Lms (  -0.0189,  12.3502, 0.11485 );
                    case 26:
                        return new Lms (  -0.024,  12.5466, 0.11544 );
                    case 27:
                        return new Lms (  -0.0289,  12.7401, 0.11604 );
                    case 28:
                        return new Lms (  -0.0337,  12.9303, 0.11664 );
                    case 29:
                        return new Lms (  -0.0385,  13.1169, 0.11723 );
                    case 30:
                        return new Lms (  -0.0431,  13.3, 0.11781 );
                    case 31:
                        return new Lms (  -0.0476,  13.4798, 0.11839 );
                    case 32:
                        return new Lms (  -0.052,  13.6567, 0.11896 );
                    case 33:
                        return new Lms (  -0.0564,  13.8309, 0.11953 );
                    case 34:
                        return new Lms (  -0.0606,  14.0031, 0.12008 );
                    case 35:
                        return new Lms (  -0.0648,  14.1736, 0.12062 );
                    case 36:
                        return new Lms (  -0.0689,  14.3429, 0.12116 );
                    case 37:
                        return new Lms (  -0.0729,  14.5113, 0.12168 );
                    case 38:
                        return new Lms (  -0.0769,  14.6791, 0.1222 );
                    case 39:
                        return new Lms (  -0.0808,  14.8466, 0.12271 );
                    case 40:
                        return new Lms (  -0.0846,  15.014, 0.12322 );
                    case 41:
                        return new Lms (  -0.0883,  15.1813, 0.12373 );
                    case 42:
                        return new Lms (  -0.092,  15.3486, 0.12425 );
                    case 43:
                        return new Lms (  -0.0957,  15.5158, 0.12478 );
                    case 44:
                        return new Lms (  -0.0993,  15.6828, 0.12531 );
                    case 45:
                        return new Lms (  -0.1028,  15.8497, 0.12586 );
                    case 46:
                        return new Lms (  -0.1063,  16.0163, 0.12643 );
                    case 47:
                        return new Lms (  -0.1097,  16.1827, 0.127 );
                    case 48:
                        return new Lms (  -0.1131,  16.3489, 0.12759 );
                    case 49:
                        return new Lms (  -0.426,  16.551, 0.11649 );
                    case 50:
                        return new Lms (  -0.448,  16.8689, 0.11717 );
                    case 51:
                        return new Lms (  -0.459,  17.033, 0.11752 );
                    case 52:
                        return new Lms (  -0.471,  17.2011, 0.11788 );
                    case 53:
                        return new Lms (  -0.482,  17.3729, 0.11826 );
                    case 54:
                        return new Lms (  -0.494,  17.548, 0.11864 );
                    case 55:
                        return new Lms (  -0.506,  17.7253, 0.11903 );
                    case 56:
                        return new Lms (  -0.518,  17.9043, 0.11942 );
                    case 57:
                        return new Lms (  -0.53,  18.086, 0.11983 );
                    case 58:
                        return new Lms (  -0.543,  18.2673, 0.12025 );
                    case 59:
                        return new Lms (  -0.555,  18.4507, 0.12067 );
                    case 60:
                        return new Lms (  -0.567,  18.633, 0.1211 );
                    case 61:
                        return new Lms (  -0.579,  18.8143, 0.12154 );
                    case 62:
                        return new Lms (  -0.591,  18.9942, 0.12198 );
                    case 63:
                        return new Lms (  -0.603,  19.172, 0.12243 );
                    case 64:
                        return new Lms (  -0.615,  19.349, 0.12289 );
                    case 65:
                        return new Lms (  -0.627,  19.5253, 0.12335 );
                    case 66:
                        return new Lms (  -0.639,  19.701, 0.12383 );
                    case 67:
                        return new Lms (  -0.651,  19.8767, 0.12432 );
                    case 68:
                        return new Lms (  -0.663,  20.0523, 0.12482 );
                    case 69:
                        return new Lms (  -0.675,  20.229, 0.12534 );
                    case 70:
                        return new Lms (  -0.687,  20.4077, 0.12587 );
                    case 71:
                        return new Lms (  -0.699,  20.5867, 0.12642 );
                    case 72:
                        return new Lms (  -0.711,  20.767, 0.12699 );
                    case 73:
                        return new Lms (  -0.723,  20.9487, 0.12757 );
                    case 74:
                        return new Lms (  -0.735,  21.1303, 0.12816 );
                    case 75:
                        return new Lms (  -0.748,  21.313, 0.12877 );
                    case 76:
                        return new Lms (  -0.76,  21.4981, 0.12939 );
                    case 77:
                        return new Lms (  -0.772,  21.6842, 0.13003 );
                    case 78:
                        return new Lms (  -0.785,  21.872, 0.13068 );
                    case 79:
                        return new Lms (  -0.797,  22.0621, 0.13136 );
                    case 80:
                        return new Lms (  -0.81,  22.2549, 0.13205 );
                    case 81:
                        return new Lms (  -0.823,  22.45, 0.13276 );
                    case 82:
                        return new Lms (  -0.836,  22.6478, 0.13349 );
                    case 83:
                        return new Lms (  -0.849,  22.848, 0.13425 );
                    case 84:
                        return new Lms (  -0.861,  23.051, 0.13502 );
                    case 85:
                        return new Lms (  -0.874,  23.2576, 0.13581 );
                    case 86:
                        return new Lms (  -0.887,  23.4657, 0.13662 );
                    case 87:
                        return new Lms (  -0.9,  23.676, 0.13744 );
                    case 88:
                        return new Lms (  -0.913,  23.8873, 0.13827 );
                    case 89:
                        return new Lms (  -0.925,  24.1001, 0.13912 );
                    case 90:
                        return new Lms (  -0.937,  24.314, 0.13998 );
                    case 91:
                        return new Lms (  -0.949,  24.5293, 0.14084 );
                    case 92:
                        return new Lms (  -0.961,  24.7467, 0.14171 );
                    case 93:
                        return new Lms (  -0.972,  24.965, 0.14259 );
                    case 94:
                        return new Lms (  -0.983,  25.1849, 0.14346 );
                    case 95:
                        return new Lms (  -0.994,  25.407, 0.14434 );
                    case 96:
                        return new Lms (  -1.005,  25.63, 0.14522 );
                    case 97:
                        return new Lms (  -1.015,  25.855, 0.14609 );
                    case 98:
                        return new Lms (  -1.024,  26.0814, 0.14696 );
                    case 99:
                        return new Lms (  -1.034,  26.308, 0.14782 );
                    case 100:
                        return new Lms (  -1.042,  26.5363, 0.14868 );
                    case 101:
                        return new Lms (  -1.051,  26.7655, 0.14953 );
                    case 102:
                        return new Lms (  -1.058,  26.996, 0.15038 );
                    case 103:
                        return new Lms (  -1.065,  27.2269, 0.15121 );
                    case 104:
                        return new Lms (  -1.072,  27.458, 0.15204 );
                    case 105:
                        return new Lms (  -1.078,  27.69, 0.15286 );
                    case 106:
                        return new Lms (  -1.083,  27.922, 0.15366 );
                    case 107:
                        return new Lms (  -1.088,  28.1557, 0.15446 );
                    case 108:
                        return new Lms (  -1.092,  28.39, 0.15525 );
                    case 109:
                        return new Lms (  -1.095,  28.6266, 0.15602 );
                    case 110:
                        return new Lms (  -1.098,  28.8637, 0.15677 );
                    case 111:
                        return new Lms (  -1.101,  29.103, 0.15751 );
                    case 112:
                        return new Lms (  -1.103,  29.343, 0.15824 );
                    case 113:
                        return new Lms (  -1.104,  29.585, 0.15895 );
                    case 114:
                        return new Lms (  -1.105,  29.828, 0.15963 );
                    case 115:
                        return new Lms (  -1.105,  30.074, 0.16031 );
                    case 116:
                        return new Lms (  -1.104,  30.323, 0.16096 );
                    case 117:
                        return new Lms (  -1.103,  30.574, 0.16161 );
                    case 118:
                        return new Lms (  -1.102,  30.829, 0.16224 );
                    case 119:
                        return new Lms (  -1.099,  31.0876, 0.16286 );
                    case 120:
                        return new Lms (  -1.097,  31.35, 0.16347 );
                    case 121:
                        return new Lms (  -1.093,  31.615, 0.16408 );
                    case 122:
                        return new Lms (  -1.089,  31.883, 0.16469 );
                    case 123:
                        return new Lms (  -1.085,  32.152, 0.16528 );
                    case 124:
                        return new Lms (  -1.08,  32.4224, 0.16588 );
                    case 125:
                        return new Lms (  -1.074,  32.6934, 0.16648 );
                    case 126:
                        return new Lms (  -1.068,  32.964, 0.16707 );
                    case 127:
                        return new Lms (  -1.061,  33.234, 0.16766 );
                    case 128:
                        return new Lms (  -1.053,  33.5036, 0.16825 );
                    case 129:
                        return new Lms (  -1.045,  33.774, 0.16884 );
                    case 130:
                        return new Lms (  -1.037,  34.0437, 0.16943 );
                    case 131:
                        return new Lms (  -1.027,  34.3143, 0.17001 );
                    case 132:
                        return new Lms (  -1.018,  34.585, 0.1706 );
                    case 133:
                        return new Lms (  -1.007,  34.8568, 0.17119 );
                    case 134:
                        return new Lms (  -0.996,  35.1298, 0.17176 );
                    case 135:
                        return new Lms (  -0.985,  35.405, 0.17233 );
                    case 136:
                        return new Lms (  -0.973,  35.6823, 0.17289 );
                    case 137:
                        return new Lms (  -0.96,  35.9633, 0.17343 );
                    case 138:
                        return new Lms (  -0.947,  36.25, 0.17397 );
                    case 139:
                        return new Lms (  -0.933,  36.5432, 0.17449 );
                    case 140:
                        return new Lms (  -0.918,  36.8423, 0.175 );
                    case 141:
                        return new Lms (  -0.903,  37.149, 0.1755 );
                    case 142:
                        return new Lms (  -0.887,  37.4638, 0.17599 );
                    case 143:
                        return new Lms (  -0.87,  37.7877, 0.17647 );
                    case 144:
                        return new Lms (  -0.852,  38.122, 0.17692 );
                    case 145:
                        return new Lms (  -0.834,  38.4653, 0.17736 );
                    case 146:
                        return new Lms (  -0.816,  38.8201, 0.17778 );
                    case 147:
                        return new Lms (  -0.796,  39.185, 0.1782 );
                    case 148:
                        return new Lms (  -0.776,  39.5619, 0.1786 );
                    case 149:
                        return new Lms (  -0.755,  39.9494, 0.179 );
                    case 150:
                        return new Lms (  -0.734,  40.348, 0.17938 );
                    case 151:
                        return new Lms (  -0.712,  40.7577, 0.17978 );
                    case 152:
                        return new Lms (  -0.69,  41.1778, 0.18017 );
                    case 153:
                        return new Lms (  -0.668,  41.609, 0.18057 );
                    case 154:
                        return new Lms (  -0.645,  42.0509, 0.18095 );
                    case 155:
                        return new Lms (  -0.622,  42.5052, 0.18135 );
                    case 156:
                        return new Lms (  -0.599,  42.97, 0.18173 );
                    case 157:
                        return new Lms (  -0.575,  43.4457, 0.1821 );
                    case 158:
                        return new Lms (  -0.552,  43.932, 0.18245 );
                    case 159:
                        return new Lms (  -0.53,  44.428, 0.18278 );
                    case 160:
                        return new Lms (  -0.507,  44.9328, 0.18307 );
                    case 161:
                        return new Lms (  -0.485,  45.4445, 0.18334 );
                    case 162:
                        return new Lms (  -0.463,  45.963, 0.18359 );
                    case 163:
                        return new Lms (  -0.442,  46.4868, 0.1838 );
                    case 164:
                        return new Lms (  -0.422,  47.0153, 0.18397 );
                    case 165:
                        return new Lms (  -0.403,  47.547, 0.18411 );
                    case 166:
                        return new Lms (  -0.384,  48.081, 0.18421 );
                    case 167:
                        return new Lms (  -0.367,  48.6173, 0.18427 );
                    case 168:
                        return new Lms (  -0.351,  49.154, 0.18428 );
                    case 169:
                        return new Lms (  -0.336,  49.6907, 0.18425 );
                    case 170:
                        return new Lms (  -0.322,  50.2259, 0.18415 );
                    case 171:
                        return new Lms (  -0.31,  50.76, 0.18399 );
                    case 172:
                        return new Lms (  -0.299,  51.2915, 0.18376 );
                    case 173:
                        return new Lms (  -0.289,  51.8192, 0.18346 );
                    case 174:
                        return new Lms (  -0.281,  52.343, 0.18309 );
                    case 175:
                        return new Lms (  -0.275,  52.8618, 0.18263 );
                    case 176:
                        return new Lms (  -0.269,  53.375, 0.18209 );
                    case 177:
                        return new Lms (  -0.266,  53.883, 0.18146 );
                    case 178:
                        return new Lms (  -0.263,  54.385, 0.18075 );
                    case 179:
                        return new Lms (  -0.263,  54.8807, 0.17995 );
                    case 180:
                        return new Lms (  -0.263,  55.369, 0.17908 );
                    case 181:
                        return new Lms (  -0.265,  55.851, 0.17813 );
                    case 182:
                        return new Lms (  -0.268,  56.3242, 0.1771 );
                    case 183:
                        return new Lms (  -0.272,  56.7897, 0.176 );
                    case 184:
                        return new Lms (  -0.278,  57.2474, 0.17482 );
                    case 185:
                        return new Lms (  -0.284,  57.6966, 0.17357 );
                    case 186:
                        return new Lms (  -0.292,  58.138, 0.17226 );
                    case 187:
                        return new Lms (  -0.3,  58.5697, 0.17087 );
                    case 188:
                        return new Lms (  -0.309,  58.9933, 0.16944 );
                    case 189:
                        return new Lms (  -0.319,  59.4074, 0.16797 );
                    case 190:
                        return new Lms (  -0.33,  59.8116, 0.16645 );
                    case 191:
                        return new Lms (  -0.341,  60.2047, 0.16491 );
                    case 192:
                        return new Lms (  -0.352,  60.588, 0.16336 );
                    case 193:
                        return new Lms (  -0.364,  60.9597, 0.1618 );
                    case 194:
                        return new Lms (  -0.376,  61.3195, 0.16025 );
                    case 195:
                        return new Lms (  -0.387,  61.668, 0.15872 );
                    case 196:
                        return new Lms (  -0.399,  62.0053, 0.15722 );
                    case 197:
                        return new Lms (  -0.411,  62.3324, 0.15576 );
                    case 198:
                        return new Lms (  -0.422,  62.647, 0.15434 );
                    case 199:
                        return new Lms (  -0.434,  62.9519, 0.15297 );
                    case 200:
                        return new Lms (  -0.445,  63.2463, 0.15166 );
                    case 201:
                        return new Lms (  -0.456,  63.531, 0.15041 );
                    case 202:
                        return new Lms (  -0.467,  63.806, 0.14922 );
                    case 203:
                        return new Lms (  -0.478,  64.0717, 0.14807 );
                    case 204:
                        return new Lms (  -0.489,  64.329, 0.14699 );
                    case 205:
                        return new Lms (  -0.5,  64.5782, 0.14596 );
                    case 206:
                        return new Lms (  -0.51,  64.8187, 0.14498 );
                    case 207:
                        return new Lms (  -0.52,  65.051, 0.14407 );
                    case 208:
                        return new Lms (  -0.53,  65.2768, 0.1432 );
                    case 209:
                        return new Lms (  -0.539,  65.4933, 0.14238 );
                    case 210:
                        return new Lms (  -0.549,  65.704, 0.14161 );
                    case 211:
                        return new Lms (  -0.558,  65.9069, 0.14088 );
                    case 212:
                        return new Lms (  -0.567,  66.1033, 0.1402 );
                    case 213:
                        return new Lms (  -0.575,  66.292, 0.13956 );
                    case 214:
                        return new Lms (  -0.583,  66.4765, 0.13896 );
                    case 215:
                        return new Lms (  -0.591,  66.6541, 0.13839 );
                    case 216:
                        return new Lms (  -0.599,  66.824, 0.13786 );
                    case 217:
                        return new Lms (  -0.606,  66.9883, 0.13735 );
                    case 218:
                        return new Lms (  -0.613,  67.147, 0.13688 );
                    case 219:
                        return new Lms (  -0.62,  67.3, 0.13643 );
                    case 220:
                        return new Lms (  -0.627,  67.448, 0.13601 );
                    case 221:
                        return new Lms (  -0.633,  67.5898, 0.13561 );
                    case 222:
                        return new Lms (  -0.639,  67.728, 0.13523 );
                    case 223:
                        return new Lms (  -0.645,  67.8607, 0.13488 );
                    case 224:
                        return new Lms (  -0.651,  67.988, 0.13453 );
                    case 225:
                        return new Lms (  -0.656,  68.111, 0.13422 );
                    case 226:
                        return new Lms (  -0.662,  68.2298, 0.13392 );
                    case 227:
                        return new Lms (  -0.667,  68.3437, 0.13363 );
                    case 228:
                        return new Lms (  -0.671,  68.454, 0.13335 );
                    case 229:
                        return new Lms (  -0.676,  68.5595, 0.1331 );
                    case 230:
                        return new Lms (  -0.68,  68.6613, 0.13284 );
                    case 231:
                        return new Lms (  -0.685,  68.76, 0.13261 );
                    case 232:
                        return new Lms (  -0.689,  68.8555, 0.13239 );
                    case 233:
                        return new Lms (  -0.693,  68.9467, 0.13217 );
                    case 234:
                        return new Lms (  -0.697,  69.036, 0.13197 );
                    case 235:
                        return new Lms (  -0.7,  69.1223, 0.13177 );
                    case 236:
                        return new Lms (  -0.704,  69.2067, 0.13158 );
                    case 237:
                        return new Lms (  -0.708,  69.289, 0.13139 );
                    case 238:
                        return new Lms (  -0.711,  69.3693, 0.13122 );
                    case 239:
                        return new Lms (  -0.714,  69.447, 0.13105 );
                    case 240:
                        return new Lms (  -0.718,  69.524, 0.13088 );
                    default:
                        throw ("ageMonths");
                }
            }
            switch (ageMonths) //Female
            {
                case 3:
                    return new Lms (  0.0402,  5.8458, 0.12619 );
                case 4:
                    return new Lms (  -0.005,  6.4237, 0.12402 );
                case 5:
                    return new Lms (  -0.043,  6.8985, 0.12274 );
                case 6:
                    return new Lms (  -0.0756,  7.297, 0.12204 );
                case 7:
                    return new Lms (  -0.1039,  7.6422, 0.12178 );
                case 8:
                    return new Lms (  -0.1288,  7.9487, 0.12181 );
                case 9:
                    return new Lms (  -0.1507,  8.2254, 0.12199 );
                case 10:
                    return new Lms (  -0.17,  8.48, 0.12223 );
                case 11:
                    return new Lms (  -0.1872,  8.7192, 0.12247 );
                case 12:
                    return new Lms (  -0.2024,  8.9481, 0.12268 );
                case 13:
                    return new Lms (  -0.2158,  9.1699, 0.12283 );
                case 14:
                    return new Lms (  -0.2278,  9.387, 0.12294 );
                case 15:
                    return new Lms (  -0.2384,  9.6008, 0.12299 );
                case 16:
                    return new Lms (  -0.2478,  9.8124, 0.12303 );
                case 17:
                    return new Lms (  -0.2562,  10.0226, 0.12306 );
                case 18:
                    return new Lms (  -0.2637,  10.2315, 0.12309 );
                case 19:
                    return new Lms (  -0.2703,  10.4393, 0.12315 );
                case 20:
                    return new Lms (  -0.2762,  10.6464, 0.12323 );
                case 21:
                    return new Lms (  -0.2815,  10.8534, 0.12335 );
                case 22:
                    return new Lms (  -0.2862,  11.0608, 0.1235 );
                case 23:
                    return new Lms (  -0.2903,  11.2688, 0.12369 );
                case 24:
                    return new Lms (  -0.2941,  11.4775, 0.1239 );
                case 25:
                    return new Lms (  -0.2975,  11.6864, 0.12414 );
                case 26:
                    return new Lms (  -0.3005,  11.8947, 0.12441 );
                case 27:
                    return new Lms (  -0.3032,  12.1015, 0.12472 );
                case 28:
                    return new Lms (  -0.3057,  12.3059, 0.12506 );
                case 29:
                    return new Lms (  -0.308,  12.5073, 0.12545 );
                case 30:
                    return new Lms (  -0.3101,  12.7055, 0.12587 );
                case 31:
                    return new Lms (  -0.312,  12.9006, 0.12633 );
                case 32:
                    return new Lms (  -0.3138,  13.093, 0.12683 );
                case 33:
                    return new Lms (  -0.3155,  13.2837, 0.12737 );
                case 34:
                    return new Lms (  -0.3171,  13.4731, 0.12794 );
                case 35:
                    return new Lms (  -0.3186,  13.6618, 0.12855 );
                case 36:
                    return new Lms (  -0.3201,  13.8503, 0.12919 );
                case 37:
                    return new Lms (  -0.3216,  14.0385, 0.12988 );
                case 38:
                    return new Lms (  -0.323,  14.2265, 0.13059 );
                case 39:
                    return new Lms (  -0.3243,  14.414, 0.13135 );
                case 40:
                    return new Lms (  -0.3257,  14.601, 0.13213 );
                case 41:
                    return new Lms (  -0.327,  14.7873, 0.13293 );
                case 42:
                    return new Lms (  -0.3283,  14.9727, 0.13376 );
                case 43:
                    return new Lms (  -0.3296,  15.1573, 0.1346 );
                case 44:
                    return new Lms (  -0.3309,  15.341, 0.13545 );
                case 45:
                    return new Lms (  -0.3322,  15.524, 0.1363 );
                case 46:
                    return new Lms (  -0.3335,  15.7064, 0.13716 );
                case 47:
                    return new Lms (  -0.3348,  15.8882, 0.138 );
                case 48:
                    return new Lms (  -0.3361,  16.0697, 0.13884 );
                case 49:
                    return new Lms (  -0.516,  16.198, 0.1244 );
                case 50:
                    return new Lms (  -0.532,  16.5359, 0.12584 );
                case 51:
                    return new Lms (  -0.539,  16.706, 0.12657 );
                case 52:
                    return new Lms (  -0.547,  16.8781, 0.12731 );
                case 53:
                    return new Lms (  -0.555,  17.0519, 0.12806 );
                case 54:
                    return new Lms (  -0.563,  17.227, 0.12881 );
                case 55:
                    return new Lms (  -0.57,  17.4041, 0.12957 );
                case 56:
                    return new Lms (  -0.578,  17.5819, 0.13033 );
                case 57:
                    return new Lms (  -0.585,  17.761, 0.1311 );
                case 58:
                    return new Lms (  -0.592,  17.9401, 0.13186 );
                case 59:
                    return new Lms (  -0.6,  18.1198, 0.13264 );
                case 60:
                    return new Lms (  -0.607,  18.299, 0.1334 );
                case 61:
                    return new Lms (  -0.614,  18.4791, 0.13416 );
                case 62:
                    return new Lms (  -0.621,  18.6581, 0.13493 );
                case 63:
                    return new Lms (  -0.627,  18.837, 0.13569 );
                case 64:
                    return new Lms (  -0.634,  19.017, 0.13643 );
                case 65:
                    return new Lms (  -0.64,  19.1967, 0.13719 );
                case 66:
                    return new Lms (  -0.647,  19.377, 0.13794 );
                case 67:
                    return new Lms (  -0.653,  19.5581, 0.13868 );
                case 68:
                    return new Lms (  -0.659,  19.7404, 0.13942 );
                case 69:
                    return new Lms (  -0.665,  19.925, 0.14016 );
                case 70:
                    return new Lms (  -0.672,  20.11, 0.1409 );
                case 71:
                    return new Lms (  -0.678,  20.2972, 0.14165 );
                case 72:
                    return new Lms (  -0.684,  20.486, 0.14239 );
                case 73:
                    return new Lms (  -0.689,  20.6778, 0.14315 );
                case 74:
                    return new Lms (  -0.695,  20.871, 0.14391 );
                case 75:
                    return new Lms (  -0.701,  21.066, 0.14468 );
                case 76:
                    return new Lms (  -0.707,  21.2629, 0.14547 );
                case 77:
                    return new Lms (  -0.712,  21.4633, 0.14626 );
                case 78:
                    return new Lms (  -0.718,  21.667, 0.14706 );
                case 79:
                    return new Lms (  -0.723,  21.8742, 0.14788 );
                case 80:
                    return new Lms (  -0.729,  22.086, 0.14872 );
                case 81:
                    return new Lms (  -0.734,  22.302, 0.14958 );
                case 82:
                    return new Lms (  -0.739,  22.5234, 0.15045 );
                case 83:
                    return new Lms (  -0.744,  22.749, 0.15134 );
                case 84:
                    return new Lms (  -0.749,  22.98, 0.15225 );
                case 85:
                    return new Lms (  -0.754,  23.2141, 0.15317 );
                case 86:
                    return new Lms (  -0.758,  23.4517, 0.15409 );
                case 87:
                    return new Lms (  -0.762,  23.691, 0.155 );
                case 88:
                    return new Lms (  -0.766,  23.933, 0.15591 );
                case 89:
                    return new Lms (  -0.77,  24.1756, 0.15682 );
                case 90:
                    return new Lms (  -0.774,  24.419, 0.15772 );
                case 91:
                    return new Lms (  -0.777,  24.6625, 0.1586 );
                case 92:
                    return new Lms (  -0.78,  24.907, 0.15947 );
                case 93:
                    return new Lms (  -0.783,  25.153, 0.16033 );
                case 94:
                    return new Lms (  -0.785,  25.398, 0.16117 );
                case 95:
                    return new Lms (  -0.787,  25.6434, 0.16201 );
                case 96:
                    return new Lms (  -0.789,  25.889, 0.16282 );
                case 97:
                    return new Lms (  -0.79,  26.1335, 0.16363 );
                case 98:
                    return new Lms (  -0.792,  26.378, 0.16443 );
                case 99:
                    return new Lms (  -0.792,  26.622, 0.16521 );
                case 100:
                    return new Lms (  -0.793,  26.8659, 0.16599 );
                case 101:
                    return new Lms (  -0.793,  27.1105, 0.16677 );
                case 102:
                    return new Lms (  -0.793,  27.355, 0.16754 );
                case 103:
                    return new Lms (  -0.793,  27.6011, 0.16832 );
                case 104:
                    return new Lms (  -0.792,  27.85, 0.16909 );
                case 105:
                    return new Lms (  -0.791,  28.101, 0.16987 );
                case 106:
                    return new Lms (  -0.789,  28.3567, 0.17064 );
                case 107:
                    return new Lms (  -0.787,  28.6159, 0.17141 );
                case 108:
                    return new Lms (  -0.785,  28.88, 0.17218 );
                case 109:
                    return new Lms (  -0.782,  29.1481, 0.17294 );
                case 110:
                    return new Lms (  -0.779,  29.4203, 0.17369 );
                case 111:
                    return new Lms (  -0.776,  29.697, 0.17445 );
                case 112:
                    return new Lms (  -0.772,  29.9771, 0.1752 );
                case 113:
                    return new Lms (  -0.768,  30.2597, 0.17594 );
                case 114:
                    return new Lms (  -0.764,  30.545, 0.17668 );
                case 115:
                    return new Lms (  -0.758,  30.8335, 0.17742 );
                case 116:
                    return new Lms (  -0.753,  31.1243, 0.17816 );
                case 117:
                    return new Lms (  -0.747,  31.417, 0.17891 );
                case 118:
                    return new Lms (  -0.74,  31.7122, 0.17967 );
                case 119:
                    return new Lms (  -0.733,  32.0096, 0.18044 );
                case 120:
                    return new Lms (  -0.725,  32.308, 0.18122 );
                case 121:
                    return new Lms (  -0.717,  32.6075, 0.18201 );
                case 122:
                    return new Lms (  -0.708,  32.9083, 0.1828 );
                case 123:
                    return new Lms (  -0.699,  33.212, 0.18359 );
                case 124:
                    return new Lms (  -0.69,  33.5147, 0.18437 );
                case 125:
                    return new Lms (  -0.68,  33.8213, 0.18514 );
                case 126:
                    return new Lms (  -0.669,  34.129, 0.18588 );
                case 127:
                    return new Lms (  -0.658,  34.4393, 0.1866 );
                case 128:
                    return new Lms (  -0.647,  34.7518, 0.18729 );
                case 129:
                    return new Lms (  -0.635,  35.068, 0.18794 );
                case 130:
                    return new Lms (  -0.623,  35.3845, 0.18855 );
                case 131:
                    return new Lms (  -0.61,  35.7048, 0.18911 );
                case 132:
                    return new Lms (  -0.597,  36.03, 0.18961 );
                case 133:
                    return new Lms (  -0.584,  36.3582, 0.19004 );
                case 134:
                    return new Lms (  -0.57,  36.6911, 0.1904 );
                case 135:
                    return new Lms (  -0.556,  37.029, 0.19066 );
                case 136:
                    return new Lms (  -0.541,  37.3731, 0.19082 );
                case 137:
                    return new Lms (  -0.526,  37.7197, 0.19089 );
                case 138:
                    return new Lms (  -0.511,  38.074, 0.19084 );
                case 139:
                    return new Lms (  -0.496,  38.4331, 0.19067 );
                case 140:
                    return new Lms (  -0.48,  38.7977, 0.19039 );
                case 141:
                    return new Lms (  -0.465,  39.168, 0.19 );
                case 142:
                    return new Lms (  -0.449,  39.5443, 0.1895 );
                case 143:
                    return new Lms (  -0.434,  39.9272, 0.18891 );
                case 144:
                    return new Lms (  -0.418,  40.316, 0.18821 );
                case 145:
                    return new Lms (  -0.403,  40.7118, 0.18743 );
                case 146:
                    return new Lms (  -0.388,  41.1136, 0.18656 );
                case 147:
                    return new Lms (  -0.374,  41.5218, 0.18563 );
                case 148:
                    return new Lms (  -0.36,  41.9359, 0.18462 );
                case 149:
                    return new Lms (  -0.347,  42.3549, 0.18355 );
                case 150:
                    return new Lms (  -0.335,  42.7779, 0.18243 );
                case 151:
                    return new Lms (  -0.324,  43.2045, 0.18128 );
                case 152:
                    return new Lms (  -0.313,  43.6328, 0.18009 );
                case 153:
                    return new Lms (  -0.304,  44.0635, 0.17887 );
                case 154:
                    return new Lms (  -0.295,  44.4952, 0.17764 );
                case 155:
                    return new Lms (  -0.288,  44.9259, 0.17641 );
                case 156:
                    return new Lms (  -0.281,  45.3568, 0.17518 );
                case 157:
                    return new Lms (  -0.276,  45.7863, 0.17395 );
                case 158:
                    return new Lms (  -0.271,  46.2121, 0.17275 );
                case 159:
                    return new Lms (  -0.268,  46.6333, 0.17156 );
                case 160:
                    return new Lms (  -0.266,  47.0457, 0.1704 );
                case 161:
                    return new Lms (  -0.265,  47.4516, 0.16924 );
                case 162:
                    return new Lms (  -0.265,  47.8494, 0.16809 );
                case 163:
                    return new Lms (  -0.266,  48.2413, 0.16693 );
                case 164:
                    return new Lms (  -0.267,  48.6218, 0.16579 );
                case 165:
                    return new Lms (  -0.27,  48.993, 0.16465 );
                case 166:
                    return new Lms (  -0.274,  49.3568, 0.1635 );
                case 167:
                    return new Lms (  -0.278,  49.7081, 0.16237 );
                case 168:
                    return new Lms (  -0.284,  50.052, 0.16124 );
                case 169:
                    return new Lms (  -0.289,  50.3846, 0.16014 );
                case 170:
                    return new Lms (  -0.296,  50.7117, 0.15904 );
                case 171:
                    return new Lms (  -0.303,  51.024, 0.15798 );
                case 172:
                    return new Lms (  -0.31,  51.3267, 0.15695 );
                case 173:
                    return new Lms (  -0.318,  51.621, 0.15595 );
                case 174:
                    return new Lms (  -0.326,  51.903, 0.15499 );
                case 175:
                    return new Lms (  -0.335,  52.176, 0.15406 );
                case 176:
                    return new Lms (  -0.343,  52.4404, 0.15316 );
                case 177:
                    return new Lms (  -0.352,  52.693, 0.15232 );
                case 178:
                    return new Lms (  -0.361,  52.9367, 0.15152 );
                case 179:
                    return new Lms (  -0.369,  53.1707, 0.15075 );
                case 180:
                    return new Lms (  -0.378,  53.3947, 0.15005 );
                case 181:
                    return new Lms (  -0.386,  53.6118, 0.14938 );
                case 182:
                    return new Lms (  -0.395,  53.8203, 0.14875 );
                case 183:
                    return new Lms (  -0.403,  54.022, 0.14817 );
                case 184:
                    return new Lms (  -0.412,  54.2157, 0.14763 );
                case 185:
                    return new Lms (  -0.42,  54.4033, 0.14713 );
                case 186:
                    return new Lms (  -0.428,  54.5842, 0.14668 );
                case 187:
                    return new Lms (  -0.436,  54.7587, 0.14625 );
                case 188:
                    return new Lms (  -0.443,  54.9271, 0.14587 );
                case 189:
                    return new Lms (  -0.451,  55.0889, 0.14551 );
                case 190:
                    return new Lms (  -0.458,  55.244, 0.14518 );
                case 191:
                    return new Lms (  -0.465,  55.3943, 0.14487 );
                case 192:
                    return new Lms (  -0.472,  55.538, 0.14459 );
                case 193:
                    return new Lms (  -0.479,  55.6765, 0.14433 );
                case 194:
                    return new Lms (  -0.485,  55.8086, 0.14409 );
                case 195:
                    return new Lms (  -0.491,  55.9355, 0.14387 );
                case 196:
                    return new Lms (  -0.497,  56.0561, 0.14366 );
                case 197:
                    return new Lms (  -0.503,  56.1713, 0.14347 );
                case 198:
                    return new Lms (  -0.508,  56.2816, 0.1433 );
                case 199:
                    return new Lms (  -0.513,  56.3863, 0.14314 );
                case 200:
                    return new Lms (  -0.518,  56.4854, 0.14299 );
                case 201:
                    return new Lms (  -0.523,  56.581, 0.14286 );
                case 202:
                    return new Lms (  -0.528,  56.6713, 0.14274 );
                case 203:
                    return new Lms (  -0.532,  56.7567, 0.14262 );
                case 204:
                    return new Lms (  -0.536,  56.838, 0.14252 );
                case 205:
                    return new Lms (  -0.54,  56.9163, 0.14243 );
                case 206:
                    return new Lms (  -0.544,  56.9892, 0.14234 );
                case 207:
                    return new Lms (  -0.547,  57.059, 0.14226 );
                case 208:
                    return new Lms (  -0.551,  57.1247, 0.14218 );
                case 209:
                    return new Lms (  -0.554,  57.1875, 0.14211 );
                case 210:
                    return new Lms (  -0.557,  57.2469, 0.14204 );
                case 211:
                    return new Lms (  -0.56,  57.3041, 0.14198 );
                case 212:
                    return new Lms (  -0.563,  57.357, 0.14192 );
                case 213:
                    return new Lms (  -0.565,  57.407, 0.14187 );
                case 214:
                    return new Lms (  -0.568,  57.4557, 0.14182 );
                case 215:
                    return new Lms (  -0.57,  57.5008, 0.14177 );
                case 216:
                    return new Lms (  -0.572,  57.544, 0.14173 );
                case 217:
                    return new Lms (  -0.574,  57.5847, 0.14168 );
                case 218:
                    return new Lms (  -0.576,  57.6235, 0.14165 );
                case 219:
                    return new Lms (  -0.578,  57.66, 0.14161 );
                case 220:
                    return new Lms (  -0.58,  57.6953, 0.14157 );
                case 221:
                    return new Lms (  -0.582,  57.7288, 0.14154 );
                case 222:
                    return new Lms (  -0.583,  57.76, 0.14151 );
                case 223:
                    return new Lms (  -0.585,  57.7904, 0.14148 );
                case 224:
                    return new Lms (  -0.586,  57.8199, 0.14145 );
                case 225:
                    return new Lms (  -0.588,  57.847, 0.14142 );
                case 226:
                    return new Lms (  -0.589,  57.8728, 0.1414 );
                case 227:
                    return new Lms (  -0.59,  57.897, 0.14137 );
                case 228:
                    return new Lms (  -0.591,  57.92, 0.14135 );
                case 229:
                    return new Lms (  -0.593,  57.9421, 0.14133 );
                case 230:
                    return new Lms (  -0.594,  57.9627, 0.14131 );
                case 231:
                    return new Lms (  -0.595,  57.982, 0.14129 );
                case 232:
                    return new Lms (  -0.595,  58.0002, 0.14127 );
                case 233:
                    return new Lms (  -0.596,  58.0164, 0.14126 );
                case 234:
                    return new Lms (  -0.597,  58.032, 0.14124 );
                case 235:
                    return new Lms (  -0.598,  58.0464, 0.14123 );
                case 236:
                    return new Lms (  -0.599,  58.0597, 0.14122 );
                case 237:
                    return new Lms (  -0.599,  58.072, 0.1412 );
                case 238:
                    return new Lms (  -0.6,  58.0833, 0.14119 );
                case 239:
                    return new Lms (  -0.6,  58.0936, 0.14118 );
                case 240:
                    return new Lms (  -0.601,  58.104, 0.14117 );
                default:
                    throw("ageMonths");
            }
        }
    })
};
