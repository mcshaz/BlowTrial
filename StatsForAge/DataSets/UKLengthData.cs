using System;
namespace StatsForAge.DataSets
{
    /// <summary>
    /// Length (or height for >2yo) in cm
    /// </summary>
    public sealed class UKLengthData : CentileData
    {
        public UKLengthData()
        {
            base.GestAgeRange = new GenderRange(25, 43);
        }
        protected override LMS LMSForGestAge(int gestAgeWeeks, bool isMale)
        {
            if (isMale)
            {
                switch (gestAgeWeeks)
                {
                    case 25:
                        return new LMS { L = 1, M = 35.42414, S = 0.08132453 };
                    case 26:
                        return new LMS { L = 1, M = 36.42492, S = 0.07862004 };
                    case 27:
                        return new LMS { L = 1, M = 37.42772, S = 0.07590725 };
                    case 28:
                        return new LMS { L = 1, M = 38.42789, S = 0.07314432 };
                    case 29:
                        return new LMS { L = 1, M = 39.43673, S = 0.07025966 };
                    case 30:
                        return new LMS { L = 1, M = 40.48008, S = 0.06722238 };
                    case 31:
                        return new LMS { L = 1, M = 41.57838, S = 0.06404407 };
                    case 32:
                        return new LMS { L = 1, M = 42.71663, S = 0.06080197 };
                    case 33:
                        return new LMS { L = 1, M = 43.87082, S = 0.05758883 };
                    case 34:
                        return new LMS { L = 1, M = 45.01804, S = 0.05449291 };
                    case 35:
                        return new LMS { L = 1, M = 46.1415, S = 0.05158876 };
                    case 36:
                        return new LMS { L = 1, M = 47.23352, S = 0.04895293 };
                    case 37:
                        return new LMS { L = 1, M = 48.28677, S = 0.0466417 };
                    case 38:
                        return new LMS { L = 1, M = 49.2766, S = 0.04469878 };
                    case 39:
                        return new LMS { L = 1, M = 50.16595, S = 0.04315059 };
                    case 40:
                        return new LMS { L = 1, M = 50.94454, S = 0.04197628 };
                    case 41:
                        return new LMS { L = 1, M = 51.64579, S = 0.04100319 };
                    case 42:
                        return new LMS { L = 1, M = 52.30513, S = 0.03994391 };
                    case 43:
                        return new LMS { L = 1, M = 53.3905, S = 0.03609 };
                    default:
                        throw new ArgumentOutOfRangeException("gestAgeWeeks");
                }
            }
            switch (gestAgeWeeks) //Female
            {
                case 25:
                    return new LMS { L = 1, M = 34.59544, S = 0.08086044 };
                case 26:
                    return new LMS { L = 1, M = 35.59771, S = 0.07735533 };
                case 27:
                    return new LMS { L = 1, M = 36.60905, S = 0.07386597 };
                case 28:
                    return new LMS { L = 1, M = 37.65832, S = 0.07042367 };
                case 29:
                    return new LMS { L = 1, M = 38.76987, S = 0.06701891 };
                case 30:
                    return new LMS { L = 1, M = 39.94117, S = 0.06362674 };
                case 31:
                    return new LMS { L = 1, M = 41.14154, S = 0.06025431 };
                case 32:
                    return new LMS { L = 1, M = 42.34725, S = 0.0569387 };
                case 33:
                    return new LMS { L = 1, M = 43.538, S = 0.05372271 };
                case 34:
                    return new LMS { L = 1, M = 44.69314, S = 0.05064634 };
                case 35:
                    return new LMS { L = 1, M = 45.79079, S = 0.04773628 };
                case 36:
                    return new LMS { L = 1, M = 46.81071, S = 0.04500635 };
                case 37:
                    return new LMS { L = 1, M = 47.73972, S = 0.04248754 };
                case 38:
                    return new LMS { L = 1, M = 48.57771, S = 0.04026448 };
                case 39:
                    return new LMS { L = 1, M = 49.33962, S = 0.03839778 };
                case 40:
                    return new LMS { L = 1, M = 50.01719, S = 0.0369674 };
                case 41:
                    return new LMS { L = 1, M = 50.62523, S = 0.03608866 };
                case 42:
                    return new LMS { L = 1, M = 51.20649, S = 0.03570984 };
                case 43:
                    return new LMS { L = 1, M = 52.4695, S = 0.03669 };
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
                        return new LMS { L = 1, M = 54.3881, S = 0.0357 };
                    case 5:
                        return new LMS { L = 1, M = 55.3374, S = 0.03534 };
                    case 6:
                        return new LMS { L = 1, M = 56.2357, S = 0.03501 };
                    case 7:
                        return new LMS { L = 1, M = 57.0851, S = 0.0347 };
                    case 8:
                        return new LMS { L = 1, M = 57.8889, S = 0.03442 };
                    case 9:
                        return new LMS { L = 1, M = 58.6536, S = 0.03416 };
                    case 10:
                        return new LMS { L = 1, M = 59.3872, S = 0.03392 };
                    case 11:
                        return new LMS { L = 1, M = 60.0894, S = 0.03369 };
                    case 12:
                        return new LMS { L = 1, M = 60.7605, S = 0.03348 };
                    case 13:
                        return new LMS { L = 1, M = 61.4013, S = 0.03329 };
                    default:
                        throw new ArgumentOutOfRangeException("ageWeeks");
                }
            }
            switch (ageWeeks)
            {
                case 4:
                    return new LMS { L = 1, M = 53.3809, S = 0.03647 };
                case 5:
                    return new LMS { L = 1, M = 54.2454, S = 0.03627 };
                case 6:
                    return new LMS { L = 1, M = 55.0642, S = 0.03609 };
                case 7:
                    return new LMS { L = 1, M = 55.8406, S = 0.03593 };
                case 8:
                    return new LMS { L = 1, M = 56.5767, S = 0.03578 };
                case 9:
                    return new LMS { L = 1, M = 57.2761, S = 0.03564 };
                case 10:
                    return new LMS { L = 1, M = 57.9436, S = 0.03552 };
                case 11:
                    return new LMS { L = 1, M = 58.5816, S = 0.0354 };
                case 12:
                    return new LMS { L = 1, M = 59.1922, S = 0.0353 };
                case 13:
                    return new LMS { L = 1, M = 59.7773, S = 0.0352 };
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
                        return new LMS { L = 1, M = 61.4292, S = 0.03328 };
                    case 4:
                        return new LMS { L = 1, M = 63.886, S = 0.03257 };
                    case 5:
                        return new LMS { L = 1, M = 65.9026, S = 0.03204 };
                    case 6:
                        return new LMS { L = 1, M = 67.6236, S = 0.03165 };
                    case 7:
                        return new LMS { L = 1, M = 69.1645, S = 0.03139 };
                    case 8:
                        return new LMS { L = 1, M = 70.5994, S = 0.03124 };
                    case 9:
                        return new LMS { L = 1, M = 71.9687, S = 0.03117 };
                    case 10:
                        return new LMS { L = 1, M = 73.2812, S = 0.03118 };
                    case 11:
                        return new LMS { L = 1, M = 74.5388, S = 0.03125 };
                    case 12:
                        return new LMS { L = 1, M = 75.7488, S = 0.03137 };
                    case 13:
                        return new LMS { L = 1, M = 76.9186, S = 0.03154 };
                    case 14:
                        return new LMS { L = 1, M = 78.0497, S = 0.03174 };
                    case 15:
                        return new LMS { L = 1, M = 79.1458, S = 0.03197 };
                    case 16:
                        return new LMS { L = 1, M = 80.2113, S = 0.03222 };
                    case 17:
                        return new LMS { L = 1, M = 81.2487, S = 0.0325 };
                    case 18:
                        return new LMS { L = 1, M = 82.2587, S = 0.03279 };
                    case 19:
                        return new LMS { L = 1, M = 83.2418, S = 0.0331 };
                    case 20:
                        return new LMS { L = 1, M = 84.1996, S = 0.03342 };
                    case 21:
                        return new LMS { L = 1, M = 85.1348, S = 0.03376 };
                    case 22:
                        return new LMS { L = 1, M = 86.0477, S = 0.0341 };
                    case 23:
                        return new LMS { L = 1, M = 86.941, S = 0.03445 };
                    case 24:
                        return new LMS { L = 1, M = 87.8161, S = 0.03479 };
                    case 25:
                        return new LMS { L = 1, M = 87.972, S = 0.03542 };
                    case 26:
                        return new LMS { L = 1, M = 88.8065, S = 0.03576 };
                    case 27:
                        return new LMS { L = 1, M = 89.6197, S = 0.0361 };
                    case 28:
                        return new LMS { L = 1, M = 90.412, S = 0.03642 };
                    case 29:
                        return new LMS { L = 1, M = 91.1828, S = 0.03674 };
                    case 30:
                        return new LMS { L = 1, M = 91.9327, S = 0.03704 };
                    case 31:
                        return new LMS { L = 1, M = 92.6631, S = 0.03733 };
                    case 32:
                        return new LMS { L = 1, M = 93.3753, S = 0.03761 };
                    case 33:
                        return new LMS { L = 1, M = 94.0711, S = 0.03787 };
                    case 34:
                        return new LMS { L = 1, M = 94.7532, S = 0.03812 };
                    case 35:
                        return new LMS { L = 1, M = 95.4236, S = 0.03836 };
                    case 36:
                        return new LMS { L = 1, M = 96.0835, S = 0.03858 };
                    case 37:
                        return new LMS { L = 1, M = 96.7337, S = 0.03879 };
                    case 38:
                        return new LMS { L = 1, M = 97.3749, S = 0.039 };
                    case 39:
                        return new LMS { L = 1, M = 98.0073, S = 0.03919 };
                    case 40:
                        return new LMS { L = 1, M = 98.631, S = 0.03937 };
                    case 41:
                        return new LMS { L = 1, M = 99.2459, S = 0.03954 };
                    case 42:
                        return new LMS { L = 1, M = 99.8515, S = 0.03971 };
                    case 43:
                        return new LMS { L = 1, M = 100.4485, S = 0.03986 };
                    case 44:
                        return new LMS { L = 1, M = 101.0374, S = 0.04002 };
                    case 45:
                        return new LMS { L = 1, M = 101.6186, S = 0.04016 };
                    case 46:
                        return new LMS { L = 1, M = 102.1933, S = 0.04031 };
                    case 47:
                        return new LMS { L = 1, M = 102.7625, S = 0.04045 };
                    case 48:
                        return new LMS { L = 1, M = 103.3273, S = 0.04059 };
                    case 49:
                        return new LMS { L = 1, M = 102.49, S = 0.04008 };
                    case 50:
                        return new LMS { L = 1, M = 103.64, S = 0.04033 };
                    case 51:
                        return new LMS { L = 1, M = 104.22, S = 0.04045 };
                    case 52:
                        return new LMS { L = 1, M = 104.8, S = 0.04057 };
                    case 53:
                        return new LMS { L = 1, M = 105.4, S = 0.04068 };
                    case 54:
                        return new LMS { L = 1, M = 106, S = 0.04078 };
                    case 55:
                        return new LMS { L = 1, M = 106.6, S = 0.04089 };
                    case 56:
                        return new LMS { L = 1, M = 107.21, S = 0.04098 };
                    case 57:
                        return new LMS { L = 1, M = 107.81, S = 0.04107 };
                    case 58:
                        return new LMS { L = 1, M = 108.41, S = 0.04115 };
                    case 59:
                        return new LMS { L = 1, M = 109.01, S = 0.04123 };
                    case 60:
                        return new LMS { L = 1, M = 109.59, S = 0.04131 };
                    case 61:
                        return new LMS { L = 1, M = 110.16, S = 0.04137 };
                    case 62:
                        return new LMS { L = 1, M = 110.73, S = 0.04144 };
                    case 63:
                        return new LMS { L = 1, M = 111.28, S = 0.04149 };
                    case 64:
                        return new LMS { L = 1, M = 111.81, S = 0.04155 };
                    case 65:
                        return new LMS { L = 1, M = 112.35, S = 0.0416 };
                    case 66:
                        return new LMS { L = 1, M = 112.87, S = 0.04165 };
                    case 67:
                        return new LMS { L = 1, M = 113.38, S = 0.0417 };
                    case 68:
                        return new LMS { L = 1, M = 113.9, S = 0.04174 };
                    case 69:
                        return new LMS { L = 1, M = 114.41, S = 0.04178 };
                    case 70:
                        return new LMS { L = 1, M = 114.92, S = 0.04182 };
                    case 71:
                        return new LMS { L = 1, M = 115.43, S = 0.04186 };
                    case 72:
                        return new LMS { L = 1, M = 115.93, S = 0.0419 };
                    case 73:
                        return new LMS { L = 1, M = 116.44, S = 0.04193 };
                    case 74:
                        return new LMS { L = 1, M = 116.94, S = 0.04197 };
                    case 75:
                        return new LMS { L = 1, M = 117.44, S = 0.042 };
                    case 76:
                        return new LMS { L = 1, M = 117.94, S = 0.04203 };
                    case 77:
                        return new LMS { L = 1, M = 118.43, S = 0.04206 };
                    case 78:
                        return new LMS { L = 1, M = 118.93, S = 0.04209 };
                    case 79:
                        return new LMS { L = 1, M = 119.42, S = 0.04212 };
                    case 80:
                        return new LMS { L = 1, M = 119.91, S = 0.04216 };
                    case 81:
                        return new LMS { L = 1, M = 120.41, S = 0.04219 };
                    case 82:
                        return new LMS { L = 1, M = 120.9, S = 0.04223 };
                    case 83:
                        return new LMS { L = 1, M = 121.4, S = 0.04227 };
                    case 84:
                        return new LMS { L = 1, M = 121.9, S = 0.04231 };
                    case 85:
                        return new LMS { L = 1, M = 122.4, S = 0.04236 };
                    case 86:
                        return new LMS { L = 1, M = 122.9, S = 0.0424 };
                    case 87:
                        return new LMS { L = 1, M = 123.4, S = 0.04245 };
                    case 88:
                        return new LMS { L = 1, M = 123.9, S = 0.04249 };
                    case 89:
                        return new LMS { L = 1, M = 124.4, S = 0.04254 };
                    case 90:
                        return new LMS { L = 1, M = 124.9, S = 0.04259 };
                    case 91:
                        return new LMS { L = 1, M = 125.4, S = 0.04263 };
                    case 92:
                        return new LMS { L = 1, M = 125.89, S = 0.04267 };
                    case 93:
                        return new LMS { L = 1, M = 126.39, S = 0.04272 };
                    case 94:
                        return new LMS { L = 1, M = 126.87, S = 0.04276 };
                    case 95:
                        return new LMS { L = 1, M = 127.36, S = 0.0428 };
                    case 96:
                        return new LMS { L = 1, M = 127.85, S = 0.04284 };
                    case 97:
                        return new LMS { L = 1, M = 128.33, S = 0.04289 };
                    case 98:
                        return new LMS { L = 1, M = 128.8, S = 0.04293 };
                    case 99:
                        return new LMS { L = 1, M = 129.27, S = 0.04297 };
                    case 100:
                        return new LMS { L = 1, M = 129.74, S = 0.04302 };
                    case 101:
                        return new LMS { L = 1, M = 130.19, S = 0.04307 };
                    case 102:
                        return new LMS { L = 1, M = 130.64, S = 0.04312 };
                    case 103:
                        return new LMS { L = 1, M = 131.09, S = 0.04318 };
                    case 104:
                        return new LMS { L = 1, M = 131.54, S = 0.04324 };
                    case 105:
                        return new LMS { L = 1, M = 131.97, S = 0.0433 };
                    case 106:
                        return new LMS { L = 1, M = 132.41, S = 0.04337 };
                    case 107:
                        return new LMS { L = 1, M = 132.84, S = 0.04344 };
                    case 108:
                        return new LMS { L = 1, M = 133.28, S = 0.04351 };
                    case 109:
                        return new LMS { L = 1, M = 133.71, S = 0.04359 };
                    case 110:
                        return new LMS { L = 1, M = 134.14, S = 0.04367 };
                    case 111:
                        return new LMS { L = 1, M = 134.57, S = 0.04376 };
                    case 112:
                        return new LMS { L = 1, M = 134.99, S = 0.04384 };
                    case 113:
                        return new LMS { L = 1, M = 135.42, S = 0.04394 };
                    case 114:
                        return new LMS { L = 1, M = 135.84, S = 0.04403 };
                    case 115:
                        return new LMS { L = 1, M = 136.26, S = 0.04413 };
                    case 116:
                        return new LMS { L = 1, M = 136.68, S = 0.04423 };
                    case 117:
                        return new LMS { L = 1, M = 137.1, S = 0.04433 };
                    case 118:
                        return new LMS { L = 1, M = 137.53, S = 0.04444 };
                    case 119:
                        return new LMS { L = 1, M = 137.96, S = 0.04456 };
                    case 120:
                        return new LMS { L = 1, M = 138.39, S = 0.04468 };
                    case 121:
                        return new LMS { L = 1, M = 138.82, S = 0.0448 };
                    case 122:
                        return new LMS { L = 1, M = 139.26, S = 0.04493 };
                    case 123:
                        return new LMS { L = 1, M = 139.69, S = 0.04506 };
                    case 124:
                        return new LMS { L = 1, M = 140.12, S = 0.0452 };
                    case 125:
                        return new LMS { L = 1, M = 140.54, S = 0.04534 };
                    case 126:
                        return new LMS { L = 1, M = 140.96, S = 0.04548 };
                    case 127:
                        return new LMS { L = 1, M = 141.37, S = 0.04562 };
                    case 128:
                        return new LMS { L = 1, M = 141.78, S = 0.04577 };
                    case 129:
                        return new LMS { L = 1, M = 142.18, S = 0.04592 };
                    case 130:
                        return new LMS { L = 1, M = 142.58, S = 0.04607 };
                    case 131:
                        return new LMS { L = 1, M = 142.98, S = 0.04623 };
                    case 132:
                        return new LMS { L = 1, M = 143.37, S = 0.04638 };
                    case 133:
                        return new LMS { L = 1, M = 143.77, S = 0.04654 };
                    case 134:
                        return new LMS { L = 1, M = 144.16, S = 0.0467 };
                    case 135:
                        return new LMS { L = 1, M = 144.55, S = 0.04687 };
                    case 136:
                        return new LMS { L = 1, M = 144.95, S = 0.04704 };
                    case 137:
                        return new LMS { L = 1, M = 145.34, S = 0.04721 };
                    case 138:
                        return new LMS { L = 1, M = 145.75, S = 0.04738 };
                    case 139:
                        return new LMS { L = 1, M = 146.16, S = 0.04755 };
                    case 140:
                        return new LMS { L = 1, M = 146.58, S = 0.04774 };
                    case 141:
                        return new LMS { L = 1, M = 147.02, S = 0.04792 };
                    case 142:
                        return new LMS { L = 1, M = 147.46, S = 0.0481 };
                    case 143:
                        return new LMS { L = 1, M = 147.91, S = 0.04829 };
                    case 144:
                        return new LMS { L = 1, M = 148.36, S = 0.04848 };
                    case 145:
                        return new LMS { L = 1, M = 148.83, S = 0.04868 };
                    case 146:
                        return new LMS { L = 1, M = 149.31, S = 0.04887 };
                    case 147:
                        return new LMS { L = 1, M = 149.81, S = 0.04906 };
                    case 148:
                        return new LMS { L = 1, M = 150.31, S = 0.04926 };
                    case 149:
                        return new LMS { L = 1, M = 150.83, S = 0.04945 };
                    case 150:
                        return new LMS { L = 1, M = 151.35, S = 0.04964 };
                    case 151:
                        return new LMS { L = 1, M = 151.89, S = 0.04983 };
                    case 152:
                        return new LMS { L = 1, M = 152.44, S = 0.05001 };
                    case 153:
                        return new LMS { L = 1, M = 153, S = 0.05019 };
                    case 154:
                        return new LMS { L = 1, M = 153.58, S = 0.05037 };
                    case 155:
                        return new LMS { L = 1, M = 154.17, S = 0.05053 };
                    case 156:
                        return new LMS { L = 1, M = 154.77, S = 0.05068 };
                    case 157:
                        return new LMS { L = 1, M = 155.38, S = 0.05083 };
                    case 158:
                        return new LMS { L = 1, M = 156, S = 0.05095 };
                    case 159:
                        return new LMS { L = 1, M = 156.63, S = 0.05107 };
                    case 160:
                        return new LMS { L = 1, M = 157.27, S = 0.05116 };
                    case 161:
                        return new LMS { L = 1, M = 157.91, S = 0.05123 };
                    case 162:
                        return new LMS { L = 1, M = 158.55, S = 0.05129 };
                    case 163:
                        return new LMS { L = 1, M = 159.2, S = 0.05132 };
                    case 164:
                        return new LMS { L = 1, M = 159.84, S = 0.05133 };
                    case 165:
                        return new LMS { L = 1, M = 160.47, S = 0.05131 };
                    case 166:
                        return new LMS { L = 1, M = 161.11, S = 0.05126 };
                    case 167:
                        return new LMS { L = 1, M = 161.74, S = 0.0512 };
                    case 168:
                        return new LMS { L = 1, M = 162.36, S = 0.0511 };
                    case 169:
                        return new LMS { L = 1, M = 162.97, S = 0.05098 };
                    case 170:
                        return new LMS { L = 1, M = 163.58, S = 0.05083 };
                    case 171:
                        return new LMS { L = 1, M = 164.17, S = 0.05065 };
                    case 172:
                        return new LMS { L = 1, M = 164.75, S = 0.05045 };
                    case 173:
                        return new LMS { L = 1, M = 165.33, S = 0.05023 };
                    case 174:
                        return new LMS { L = 1, M = 165.88, S = 0.04998 };
                    case 175:
                        return new LMS { L = 1, M = 166.43, S = 0.04971 };
                    case 176:
                        return new LMS { L = 1, M = 166.96, S = 0.04942 };
                    case 177:
                        return new LMS { L = 1, M = 167.48, S = 0.04912 };
                    case 178:
                        return new LMS { L = 1, M = 167.98, S = 0.04879 };
                    case 179:
                        return new LMS { L = 1, M = 168.47, S = 0.04846 };
                    case 180:
                        return new LMS { L = 1, M = 168.94, S = 0.04811 };
                    case 181:
                        return new LMS { L = 1, M = 169.4, S = 0.04776 };
                    case 182:
                        return new LMS { L = 1, M = 169.84, S = 0.0474 };
                    case 183:
                        return new LMS { L = 1, M = 170.26, S = 0.04703 };
                    case 184:
                        return new LMS { L = 1, M = 170.67, S = 0.04666 };
                    case 185:
                        return new LMS { L = 1, M = 171.06, S = 0.0463 };
                    case 186:
                        return new LMS { L = 1, M = 171.43, S = 0.04594 };
                    case 187:
                        return new LMS { L = 1, M = 171.79, S = 0.04558 };
                    case 188:
                        return new LMS { L = 1, M = 172.14, S = 0.04522 };
                    case 189:
                        return new LMS { L = 1, M = 172.48, S = 0.04488 };
                    case 190:
                        return new LMS { L = 1, M = 172.79, S = 0.04454 };
                    case 191:
                        return new LMS { L = 1, M = 173.1, S = 0.04421 };
                    case 192:
                        return new LMS { L = 1, M = 173.39, S = 0.04389 };
                    case 193:
                        return new LMS { L = 1, M = 173.66, S = 0.04358 };
                    case 194:
                        return new LMS { L = 1, M = 173.93, S = 0.04328 };
                    case 195:
                        return new LMS { L = 1, M = 174.18, S = 0.043 };
                    case 196:
                        return new LMS { L = 1, M = 174.42, S = 0.04273 };
                    case 197:
                        return new LMS { L = 1, M = 174.64, S = 0.04246 };
                    case 198:
                        return new LMS { L = 1, M = 174.86, S = 0.04222 };
                    case 199:
                        return new LMS { L = 1, M = 175.06, S = 0.04198 };
                    case 200:
                        return new LMS { L = 1, M = 175.25, S = 0.04175 };
                    case 201:
                        return new LMS { L = 1, M = 175.43, S = 0.04154 };
                    case 202:
                        return new LMS { L = 1, M = 175.61, S = 0.04134 };
                    case 203:
                        return new LMS { L = 1, M = 175.77, S = 0.04114 };
                    case 204:
                        return new LMS { L = 1, M = 175.92, S = 0.04096 };
                    case 205:
                        return new LMS { L = 1, M = 176.07, S = 0.04079 };
                    case 206:
                        return new LMS { L = 1, M = 176.21, S = 0.04063 };
                    case 207:
                        return new LMS { L = 1, M = 176.33, S = 0.04047 };
                    case 208:
                        return new LMS { L = 1, M = 176.45, S = 0.04033 };
                    case 209:
                        return new LMS { L = 1, M = 176.57, S = 0.0402 };
                    case 210:
                        return new LMS { L = 1, M = 176.67, S = 0.04008 };
                    case 211:
                        return new LMS { L = 1, M = 176.76, S = 0.03997 };
                    case 212:
                        return new LMS { L = 1, M = 176.85, S = 0.03987 };
                    case 213:
                        return new LMS { L = 1, M = 176.92, S = 0.03979 };
                    case 214:
                        return new LMS { L = 1, M = 176.99, S = 0.03971 };
                    case 215:
                        return new LMS { L = 1, M = 177.04, S = 0.03964 };
                    case 216:
                        return new LMS { L = 1, M = 177.09, S = 0.03958 };
                    case 217:
                        return new LMS { L = 1, M = 177.13, S = 0.03953 };
                    case 218:
                        return new LMS { L = 1, M = 177.17, S = 0.03948 };
                    case 219:
                        return new LMS { L = 1, M = 177.2, S = 0.03945 };
                    case 220:
                        return new LMS { L = 1, M = 177.23, S = 0.03942 };
                    case 221:
                        return new LMS { L = 1, M = 177.25, S = 0.0394 };
                    case 222:
                        return new LMS { L = 1, M = 177.26, S = 0.03938 };
                    case 223:
                        return new LMS { L = 1, M = 177.27, S = 0.03937 };
                    case 224:
                        return new LMS { L = 1, M = 177.27, S = 0.03936 };
                    case 225:
                        return new LMS { L = 1, M = 177.28, S = 0.03936 };
                    case 226:
                        return new LMS { L = 1, M = 177.28, S = 0.03936 };
                    case 227:
                        return new LMS { L = 1, M = 177.28, S = 0.03936 };
                    case 228:
                        return new LMS { L = 1, M = 177.28, S = 0.03935 };
                    case 229:
                        return new LMS { L = 1, M = 177.29, S = 0.03935 };
                    case 230:
                        return new LMS { L = 1, M = 177.29, S = 0.03934 };
                    case 231:
                        return new LMS { L = 1, M = 177.29, S = 0.03934 };
                    case 232:
                        return new LMS { L = 1, M = 177.3, S = 0.03934 };
                    case 233:
                        return new LMS { L = 1, M = 177.3, S = 0.03934 };
                    case 234:
                        return new LMS { L = 1, M = 177.3, S = 0.03934 };
                    case 235:
                        return new LMS { L = 1, M = 177.3, S = 0.03933 };
                    case 236:
                        return new LMS { L = 1, M = 177.3, S = 0.03933 };
                    case 237:
                        return new LMS { L = 1, M = 177.31, S = 0.03932 };
                    case 238:
                        return new LMS { L = 1, M = 177.32, S = 0.03931 };
                    case 239:
                        return new LMS { L = 1, M = 177.33, S = 0.0393 };
                    case 240:
                        return new LMS { L = 1, M = 177.34, S = 0.03929 };
                    default:
                        throw new ArgumentOutOfRangeException("ageMonths");
                }
            }
            switch (ageMonths)
            {
                case 3:
                    return new LMS { L = 1, M = 59.8029, S = 0.0352 };
                case 4:
                    return new LMS { L = 1, M = 62.0899, S = 0.03486 };
                case 5:
                    return new LMS { L = 1, M = 64.0301, S = 0.03463 };
                case 6:
                    return new LMS { L = 1, M = 65.7311, S = 0.03448 };
                case 7:
                    return new LMS { L = 1, M = 67.2873, S = 0.03441 };
                case 8:
                    return new LMS { L = 1, M = 68.7498, S = 0.0344 };
                case 9:
                    return new LMS { L = 1, M = 70.1435, S = 0.03444 };
                case 10:
                    return new LMS { L = 1, M = 71.4818, S = 0.03452 };
                case 11:
                    return new LMS { L = 1, M = 72.771, S = 0.03464 };
                case 12:
                    return new LMS { L = 1, M = 74.015, S = 0.03479 };
                case 13:
                    return new LMS { L = 1, M = 75.2176, S = 0.03496 };
                case 14:
                    return new LMS { L = 1, M = 76.3817, S = 0.03514 };
                case 15:
                    return new LMS { L = 1, M = 77.5099, S = 0.03534 };
                case 16:
                    return new LMS { L = 1, M = 78.6055, S = 0.03555 };
                case 17:
                    return new LMS { L = 1, M = 79.671, S = 0.03576 };
                case 18:
                    return new LMS { L = 1, M = 80.7079, S = 0.03598 };
                case 19:
                    return new LMS { L = 1, M = 81.7182, S = 0.0362 };
                case 20:
                    return new LMS { L = 1, M = 82.7036, S = 0.03643 };
                case 21:
                    return new LMS { L = 1, M = 83.6654, S = 0.03666 };
                case 22:
                    return new LMS { L = 1, M = 84.604, S = 0.03688 };
                case 23:
                    return new LMS { L = 1, M = 85.5202, S = 0.03711 };
                case 24:
                    return new LMS { L = 1, M = 86.4153, S = 0.03734 };
                case 25:
                    return new LMS { L = 1, M = 86.5904, S = 0.03786 };
                case 26:
                    return new LMS { L = 1, M = 87.4462, S = 0.03808 };
                case 27:
                    return new LMS { L = 1, M = 88.283, S = 0.0383 };
                case 28:
                    return new LMS { L = 1, M = 89.1004, S = 0.03851 };
                case 29:
                    return new LMS { L = 1, M = 89.8991, S = 0.03872 };
                case 30:
                    return new LMS { L = 1, M = 90.6797, S = 0.03893 };
                case 31:
                    return new LMS { L = 1, M = 91.443, S = 0.03913 };
                case 32:
                    return new LMS { L = 1, M = 92.1906, S = 0.03933 };
                case 33:
                    return new LMS { L = 1, M = 92.9239, S = 0.03952 };
                case 34:
                    return new LMS { L = 1, M = 93.6444, S = 0.03971 };
                case 35:
                    return new LMS { L = 1, M = 94.3533, S = 0.03989 };
                case 36:
                    return new LMS { L = 1, M = 95.0515, S = 0.04006 };
                case 37:
                    return new LMS { L = 1, M = 95.7399, S = 0.04024 };
                case 38:
                    return new LMS { L = 1, M = 96.4187, S = 0.04041 };
                case 39:
                    return new LMS { L = 1, M = 97.0885, S = 0.04057 };
                case 40:
                    return new LMS { L = 1, M = 97.7493, S = 0.04073 };
                case 41:
                    return new LMS { L = 1, M = 98.4015, S = 0.04089 };
                case 42:
                    return new LMS { L = 1, M = 99.0448, S = 0.04105 };
                case 43:
                    return new LMS { L = 1, M = 99.6795, S = 0.0412 };
                case 44:
                    return new LMS { L = 1, M = 100.3058, S = 0.04135 };
                case 45:
                    return new LMS { L = 1, M = 100.9238, S = 0.0415 };
                case 46:
                    return new LMS { L = 1, M = 101.5337, S = 0.04164 };
                case 47:
                    return new LMS { L = 1, M = 102.136, S = 0.04179 };
                case 48:
                    return new LMS { L = 1, M = 102.7312, S = 0.04193 };
                case 49:
                    return new LMS { L = 1, M = 101.54, S = 0.03967 };
                case 50:
                    return new LMS { L = 1, M = 102.71, S = 0.03992 };
                case 51:
                    return new LMS { L = 1, M = 103.31, S = 0.04004 };
                case 52:
                    return new LMS { L = 1, M = 103.91, S = 0.04016 };
                case 53:
                    return new LMS { L = 1, M = 104.53, S = 0.04028 };
                case 54:
                    return new LMS { L = 1, M = 105.15, S = 0.04041 };
                case 55:
                    return new LMS { L = 1, M = 105.77, S = 0.04053 };
                case 56:
                    return new LMS { L = 1, M = 106.4, S = 0.04065 };
                case 57:
                    return new LMS { L = 1, M = 107.02, S = 0.04077 };
                case 58:
                    return new LMS { L = 1, M = 107.64, S = 0.04088 };
                case 59:
                    return new LMS { L = 1, M = 108.26, S = 0.04099 };
                case 60:
                    return new LMS { L = 1, M = 108.86, S = 0.0411 };
                case 61:
                    return new LMS { L = 1, M = 109.45, S = 0.0412 };
                case 62:
                    return new LMS { L = 1, M = 110.03, S = 0.0413 };
                case 63:
                    return new LMS { L = 1, M = 110.6, S = 0.0414 };
                case 64:
                    return new LMS { L = 1, M = 111.16, S = 0.04149 };
                case 65:
                    return new LMS { L = 1, M = 111.71, S = 0.04157 };
                case 66:
                    return new LMS { L = 1, M = 112.24, S = 0.04165 };
                case 67:
                    return new LMS { L = 1, M = 112.77, S = 0.04173 };
                case 68:
                    return new LMS { L = 1, M = 113.29, S = 0.0418 };
                case 69:
                    return new LMS { L = 1, M = 113.81, S = 0.04187 };
                case 70:
                    return new LMS { L = 1, M = 114.32, S = 0.04194 };
                case 71:
                    return new LMS { L = 1, M = 114.82, S = 0.042 };
                case 72:
                    return new LMS { L = 1, M = 115.33, S = 0.04206 };
                case 73:
                    return new LMS { L = 1, M = 115.83, S = 0.04212 };
                case 74:
                    return new LMS { L = 1, M = 116.33, S = 0.04217 };
                case 75:
                    return new LMS { L = 1, M = 116.82, S = 0.04222 };
                case 76:
                    return new LMS { L = 1, M = 117.31, S = 0.04227 };
                case 77:
                    return new LMS { L = 1, M = 117.81, S = 0.04232 };
                case 78:
                    return new LMS { L = 1, M = 118.3, S = 0.04237 };
                case 79:
                    return new LMS { L = 1, M = 118.79, S = 0.04241 };
                case 80:
                    return new LMS { L = 1, M = 119.28, S = 0.04246 };
                case 81:
                    return new LMS { L = 1, M = 119.77, S = 0.0425 };
                case 82:
                    return new LMS { L = 1, M = 120.26, S = 0.04254 };
                case 83:
                    return new LMS { L = 1, M = 120.77, S = 0.04258 };
                case 84:
                    return new LMS { L = 1, M = 121.27, S = 0.04261 };
                case 85:
                    return new LMS { L = 1, M = 121.77, S = 0.04265 };
                case 86:
                    return new LMS { L = 1, M = 122.28, S = 0.04268 };
                case 87:
                    return new LMS { L = 1, M = 122.79, S = 0.04271 };
                case 88:
                    return new LMS { L = 1, M = 123.31, S = 0.04273 };
                case 89:
                    return new LMS { L = 1, M = 123.82, S = 0.04276 };
                case 90:
                    return new LMS { L = 1, M = 124.33, S = 0.04278 };
                case 91:
                    return new LMS { L = 1, M = 124.84, S = 0.04281 };
                case 92:
                    return new LMS { L = 1, M = 125.34, S = 0.04284 };
                case 93:
                    return new LMS { L = 1, M = 125.85, S = 0.04287 };
                case 94:
                    return new LMS { L = 1, M = 126.35, S = 0.0429 };
                case 95:
                    return new LMS { L = 1, M = 126.85, S = 0.04293 };
                case 96:
                    return new LMS { L = 1, M = 127.34, S = 0.04298 };
                case 97:
                    return new LMS { L = 1, M = 127.82, S = 0.04302 };
                case 98:
                    return new LMS { L = 1, M = 128.3, S = 0.04308 };
                case 99:
                    return new LMS { L = 1, M = 128.76, S = 0.04314 };
                case 100:
                    return new LMS { L = 1, M = 129.23, S = 0.04321 };
                case 101:
                    return new LMS { L = 1, M = 129.69, S = 0.04328 };
                case 102:
                    return new LMS { L = 1, M = 130.14, S = 0.04336 };
                case 103:
                    return new LMS { L = 1, M = 130.59, S = 0.04345 };
                case 104:
                    return new LMS { L = 1, M = 131.03, S = 0.04354 };
                case 105:
                    return new LMS { L = 1, M = 131.48, S = 0.04364 };
                case 106:
                    return new LMS { L = 1, M = 131.92, S = 0.04375 };
                case 107:
                    return new LMS { L = 1, M = 132.37, S = 0.04387 };
                case 108:
                    return new LMS { L = 1, M = 132.82, S = 0.04399 };
                case 109:
                    return new LMS { L = 1, M = 133.27, S = 0.04412 };
                case 110:
                    return new LMS { L = 1, M = 133.73, S = 0.04426 };
                case 111:
                    return new LMS { L = 1, M = 134.19, S = 0.0444 };
                case 112:
                    return new LMS { L = 1, M = 134.65, S = 0.04455 };
                case 113:
                    return new LMS { L = 1, M = 135.12, S = 0.04471 };
                case 114:
                    return new LMS { L = 1, M = 135.59, S = 0.04487 };
                case 115:
                    return new LMS { L = 1, M = 136.06, S = 0.04504 };
                case 116:
                    return new LMS { L = 1, M = 136.53, S = 0.04521 };
                case 117:
                    return new LMS { L = 1, M = 137, S = 0.04538 };
                case 118:
                    return new LMS { L = 1, M = 137.48, S = 0.04556 };
                case 119:
                    return new LMS { L = 1, M = 137.95, S = 0.04574 };
                case 120:
                    return new LMS { L = 1, M = 138.43, S = 0.04593 };
                case 121:
                    return new LMS { L = 1, M = 138.9, S = 0.04612 };
                case 122:
                    return new LMS { L = 1, M = 139.38, S = 0.0463 };
                case 123:
                    return new LMS { L = 1, M = 139.86, S = 0.04649 };
                case 124:
                    return new LMS { L = 1, M = 140.33, S = 0.04667 };
                case 125:
                    return new LMS { L = 1, M = 140.81, S = 0.04685 };
                case 126:
                    return new LMS { L = 1, M = 141.28, S = 0.04702 };
                case 127:
                    return new LMS { L = 1, M = 141.76, S = 0.04719 };
                case 128:
                    return new LMS { L = 1, M = 142.23, S = 0.04734 };
                case 129:
                    return new LMS { L = 1, M = 142.71, S = 0.04749 };
                case 130:
                    return new LMS { L = 1, M = 143.18, S = 0.04762 };
                case 131:
                    return new LMS { L = 1, M = 143.65, S = 0.04774 };
                case 132:
                    return new LMS { L = 1, M = 144.12, S = 0.04785 };
                case 133:
                    return new LMS { L = 1, M = 144.59, S = 0.04793 };
                case 134:
                    return new LMS { L = 1, M = 145.06, S = 0.048 };
                case 135:
                    return new LMS { L = 1, M = 145.53, S = 0.04805 };
                case 136:
                    return new LMS { L = 1, M = 146, S = 0.04808 };
                case 137:
                    return new LMS { L = 1, M = 146.47, S = 0.04809 };
                case 138:
                    return new LMS { L = 1, M = 146.93, S = 0.04808 };
                case 139:
                    return new LMS { L = 1, M = 147.4, S = 0.04805 };
                case 140:
                    return new LMS { L = 1, M = 147.87, S = 0.04799 };
                case 141:
                    return new LMS { L = 1, M = 148.34, S = 0.04792 };
                case 142:
                    return new LMS { L = 1, M = 148.82, S = 0.04782 };
                case 143:
                    return new LMS { L = 1, M = 149.28, S = 0.0477 };
                case 144:
                    return new LMS { L = 1, M = 149.76, S = 0.04755 };
                case 145:
                    return new LMS { L = 1, M = 150.24, S = 0.04739 };
                case 146:
                    return new LMS { L = 1, M = 150.71, S = 0.04721 };
                case 147:
                    return new LMS { L = 1, M = 151.18, S = 0.04701 };
                case 148:
                    return new LMS { L = 1, M = 151.66, S = 0.0468 };
                case 149:
                    return new LMS { L = 1, M = 152.13, S = 0.04657 };
                case 150:
                    return new LMS { L = 1, M = 152.6, S = 0.04632 };
                case 151:
                    return new LMS { L = 1, M = 153.06, S = 0.04606 };
                case 152:
                    return new LMS { L = 1, M = 153.52, S = 0.04579 };
                case 153:
                    return new LMS { L = 1, M = 153.96, S = 0.04551 };
                case 154:
                    return new LMS { L = 1, M = 154.41, S = 0.04522 };
                case 155:
                    return new LMS { L = 1, M = 154.85, S = 0.04492 };
                case 156:
                    return new LMS { L = 1, M = 155.28, S = 0.04462 };
                case 157:
                    return new LMS { L = 1, M = 155.7, S = 0.04431 };
                case 158:
                    return new LMS { L = 1, M = 156.11, S = 0.044 };
                case 159:
                    return new LMS { L = 1, M = 156.52, S = 0.04369 };
                case 160:
                    return new LMS { L = 1, M = 156.91, S = 0.04338 };
                case 161:
                    return new LMS { L = 1, M = 157.28, S = 0.04307 };
                case 162:
                    return new LMS { L = 1, M = 157.65, S = 0.04276 };
                case 163:
                    return new LMS { L = 1, M = 158, S = 0.04245 };
                case 164:
                    return new LMS { L = 1, M = 158.35, S = 0.04215 };
                case 165:
                    return new LMS { L = 1, M = 158.69, S = 0.04185 };
                case 166:
                    return new LMS { L = 1, M = 159.01, S = 0.04156 };
                case 167:
                    return new LMS { L = 1, M = 159.31, S = 0.04127 };
                case 168:
                    return new LMS { L = 1, M = 159.61, S = 0.041 };
                case 169:
                    return new LMS { L = 1, M = 159.89, S = 0.04073 };
                case 170:
                    return new LMS { L = 1, M = 160.16, S = 0.04046 };
                case 171:
                    return new LMS { L = 1, M = 160.42, S = 0.04021 };
                case 172:
                    return new LMS { L = 1, M = 160.67, S = 0.03997 };
                case 173:
                    return new LMS { L = 1, M = 160.9, S = 0.03974 };
                case 174:
                    return new LMS { L = 1, M = 161.12, S = 0.03952 };
                case 175:
                    return new LMS { L = 1, M = 161.32, S = 0.03931 };
                case 176:
                    return new LMS { L = 1, M = 161.52, S = 0.0391 };
                case 177:
                    return new LMS { L = 1, M = 161.71, S = 0.03892 };
                case 178:
                    return new LMS { L = 1, M = 161.88, S = 0.03874 };
                case 179:
                    return new LMS { L = 1, M = 162.03, S = 0.03858 };
                case 180:
                    return new LMS { L = 1, M = 162.18, S = 0.03842 };
                case 181:
                    return new LMS { L = 1, M = 162.32, S = 0.03828 };
                case 182:
                    return new LMS { L = 1, M = 162.44, S = 0.03815 };
                case 183:
                    return new LMS { L = 1, M = 162.56, S = 0.03803 };
                case 184:
                    return new LMS { L = 1, M = 162.66, S = 0.03792 };
                case 185:
                    return new LMS { L = 1, M = 162.76, S = 0.03782 };
                case 186:
                    return new LMS { L = 1, M = 162.85, S = 0.03772 };
                case 187:
                    return new LMS { L = 1, M = 162.92, S = 0.03764 };
                case 188:
                    return new LMS { L = 1, M = 163, S = 0.03756 };
                case 189:
                    return new LMS { L = 1, M = 163.06, S = 0.03749 };
                case 190:
                    return new LMS { L = 1, M = 163.12, S = 0.03743 };
                case 191:
                    return new LMS { L = 1, M = 163.18, S = 0.03737 };
                case 192:
                    return new LMS { L = 1, M = 163.22, S = 0.03732 };
                case 193:
                    return new LMS { L = 1, M = 163.27, S = 0.03728 };
                case 194:
                    return new LMS { L = 1, M = 163.3, S = 0.03723 };
                case 195:
                    return new LMS { L = 1, M = 163.34, S = 0.0372 };
                case 196:
                    return new LMS { L = 1, M = 163.37, S = 0.03716 };
                case 197:
                    return new LMS { L = 1, M = 163.4, S = 0.03713 };
                case 198:
                    return new LMS { L = 1, M = 163.42, S = 0.03711 };
                case 199:
                    return new LMS { L = 1, M = 163.45, S = 0.03708 };
                case 200:
                    return new LMS { L = 1, M = 163.46, S = 0.03707 };
                case 201:
                    return new LMS { L = 1, M = 163.48, S = 0.03705 };
                case 202:
                    return new LMS { L = 1, M = 163.49, S = 0.03704 };
                case 203:
                    return new LMS { L = 1, M = 163.5, S = 0.03703 };
                case 204:
                    return new LMS { L = 1, M = 163.51, S = 0.03702 };
                case 205:
                    return new LMS { L = 1, M = 163.51, S = 0.03701 };
                case 206:
                    return new LMS { L = 1, M = 163.52, S = 0.03701 };
                case 207:
                    return new LMS { L = 1, M = 163.52, S = 0.03701 };
                case 208:
                    return new LMS { L = 1, M = 163.52, S = 0.03701 };
                case 209:
                    return new LMS { L = 1, M = 163.52, S = 0.037 };
                case 210:
                    return new LMS { L = 1, M = 163.53, S = 0.037 };
                case 211:
                    return new LMS { L = 1, M = 163.53, S = 0.03699 };
                case 212:
                    return new LMS { L = 1, M = 163.54, S = 0.03699 };
                case 213:
                    return new LMS { L = 1, M = 163.55, S = 0.03698 };
                case 214:
                    return new LMS { L = 1, M = 163.56, S = 0.03697 };
                case 215:
                    return new LMS { L = 1, M = 163.57, S = 0.03696 };
                case 216:
                    return new LMS { L = 1, M = 163.57, S = 0.03695 };
                case 217:
                    return new LMS { L = 1, M = 163.58, S = 0.03695 };
                case 218:
                    return new LMS { L = 1, M = 163.59, S = 0.03694 };
                case 219:
                    return new LMS { L = 1, M = 163.6, S = 0.03693 };
                case 220:
                    return new LMS { L = 1, M = 163.6, S = 0.03693 };
                case 221:
                    return new LMS { L = 1, M = 163.61, S = 0.03692 };
                case 222:
                    return new LMS { L = 1, M = 163.61, S = 0.03692 };
                case 223:
                    return new LMS { L = 1, M = 163.62, S = 0.03691 };
                case 224:
                    return new LMS { L = 1, M = 163.62, S = 0.03691 };
                case 225:
                    return new LMS { L = 1, M = 163.62, S = 0.03691 };
                case 226:
                    return new LMS { L = 1, M = 163.63, S = 0.03691 };
                case 227:
                    return new LMS { L = 1, M = 163.63, S = 0.03691 };
                case 228:
                    return new LMS { L = 1, M = 163.63, S = 0.03691 };
                case 229:
                    return new LMS { L = 1, M = 163.63, S = 0.03691 };
                case 230:
                    return new LMS { L = 1, M = 163.63, S = 0.03691 };
                case 231:
                    return new LMS { L = 1, M = 163.63, S = 0.03691 };
                case 232:
                    return new LMS { L = 1, M = 163.63, S = 0.03691 };
                case 233:
                    return new LMS { L = 1, M = 163.63, S = 0.03691 };
                case 234:
                    return new LMS { L = 1, M = 163.63, S = 0.0369 };
                case 235:
                    return new LMS { L = 1, M = 163.63, S = 0.0369 };
                case 236:
                    return new LMS { L = 1, M = 163.63, S = 0.0369 };
                case 237:
                    return new LMS { L = 1, M = 163.63, S = 0.0369 };
                case 238:
                    return new LMS { L = 1, M = 163.64, S = 0.0369 };
                case 239:
                    return new LMS { L = 1, M = 163.64, S = 0.0369 };
                case 240:
                    return new LMS { L = 1, M = 163.64, S = 0.0369 };
                default:
                    throw new ArgumentOutOfRangeException("ageMonths");
            }
        }
    }
}

