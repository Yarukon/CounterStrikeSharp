using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace CounterStrikeSharp.API.Modules.Utils
{
    public class CEntityKeyValues
    {
        internal Dictionary<string, KeyValueContainer> KeyValues = new();

        internal List<nint> KeyValuePtrs = [];
        internal List<nint> StringPtrs = [];

        // Getter
        public bool GetBool(string key, bool defaultValue = false) => GetValue(key, defaultValue);
        public int GetInt(string key, int defaultValue = 0) => GetValue(key, defaultValue);
        public uint GetUInt(string key, uint defaultValue = 0) => GetValue(key, defaultValue);
        public long GetInt64(string key, long defaultValue = 0) => GetValue(key, defaultValue);
        public ulong GetUInt64(string key, ulong defaultValue = 0) => GetValue(key, defaultValue);
        public float GetFloat(string key, float defaultValue = 0) => GetValue(key, defaultValue);
        public double GetDouble(string key, double defaultValue = 0) => GetValue(key, defaultValue);
        public string GetString(string key, string defaultValue = "") => GetValue(key, defaultValue);
        public nint GetPointer(string key, nint defaultValue = 0) => GetValue(key, defaultValue);
        public uint GetStringToken(string key, uint defaultValue = 0) => GetValue(key, defaultValue);
        public CEntityHandle? GetEHandle(string key, CEntityHandle? defaultValue = null) => GetValue(key, defaultValue);
        public Color GetColor(string key) => GetValue(key, Color.Empty);
        public Vector3? GetVector(string key, Vector3? defaultValue = null) => GetValue(key, defaultValue);
        public Vector2? GetVector2D(string key, Vector2? defaultValue = null) => GetValue(key, defaultValue);
        public Vector4? GetVector4D(string key, Vector4? defaultValue = null) => GetValue(key, defaultValue);
        public Vector4? GetQuaternion(string key, Vector4? defaultValue = null) => GetValue(key, defaultValue);
        public EKVAngle? GetAngle(string key, EKVAngle? defaultValue = null) => GetValue(key, defaultValue);
        public Matrix3x4? GetMatrix3x4(string key, Matrix3x4? defaultValue = null) => GetValue(key, defaultValue);

        // Setter
        public void SetBool(string key, bool value) => SetValue<bool>(key, KeyValuesType.TYPE_BOOL, value);
        public void SetInt(string key, int value) => SetValue<int>(key, KeyValuesType.TYPE_INT, value);
        public void SetUInt(string key, uint value) => SetValue<uint>(key, KeyValuesType.TYPE_UINT, value);
        public void SetInt64(string key, long value) => SetValue<long>(key, KeyValuesType.TYPE_INT64, value);
        public void SetUInt64(string key, ulong value) => SetValue<ulong>(key, KeyValuesType.TYPE_UINT64, value);
        public void SetFloat(string key, float value) => SetValue<float>(key, KeyValuesType.TYPE_FLOAT, value);
        public void SetDouble(string key, double value) => SetValue<double>(key, KeyValuesType.TYPE_DOUBLE, value);
        public void SetString(string key, string value) => SetValue<string>(key, KeyValuesType.TYPE_STRING, value);
        public void SetPointer(string key, nint value) => SetValue<nint>(key, KeyValuesType.TYPE_POINTER, value);
        public void SetStringToken(string key, uint value) => SetValue<uint>(key, KeyValuesType.TYPE_STRING_TOKEN, value); // Token is integer
        public void SetEHandle(string key, CEntityHandle value) => SetValue<CEntityHandle>(key, KeyValuesType.TYPE_EHANDLE, value);
        public void SetColor(string key, Color value) => SetValue<Color>(key, KeyValuesType.TYPE_COLOR, value);
        public void SetVector(string key, float x, float y, float z) => SetValue<Vector3>(key, KeyValuesType.TYPE_VECTOR, new Vector3(x, y, z));
        public void SetVector2D(string key, float x, float y) => SetValue<Vector2>(key, KeyValuesType.TYPE_VECTOR2D, new Vector2(x, y));
        public void SetVector4D(string key, float x, float y, float z, float w) => SetValue<Vector4>(key, KeyValuesType.TYPE_VECTOR4D, new Vector4(x, y, z, w));
        public void SetQuaternion(string key, float x, float y, float z, float w) => SetValue<Vector4>(key, KeyValuesType.TYPE_QUATERNION, new Vector4(x, y, z, w)); // Same class with Vector4D
        public void SetAngle(string key, float pitch, float yaw, float roll) => SetValue<EKVAngle>(key, KeyValuesType.TYPE_QANGLE, new EKVAngle(pitch, yaw, roll));
        public void SetMatrix3x4(string key, Matrix3x4 value) => SetValue<Matrix3x4>(key, KeyValuesType.TYPE_MATRIX3X4, value);

        public bool Remove(string key) => KeyValues.Remove(key);
        public void RemoveAll() => KeyValues.Clear();
        public int Count() => KeyValues.Count;

        internal void SetValue<T>(string key, KeyValuesType type, object value)
        {
            if (value == null)
                throw new ArgumentNullException("Value can't be null!");

            if (KeyValues.TryGetValue(key, out KeyValueContainer? v))
                v.Set(value);
            else
            {
                KeyValueContainer container = new(type, value);
                KeyValues.Add(key, container);
            }
        }

        internal T GetValue<T>(string key, T defaultValue)
        {
            if (KeyValues.TryGetValue(key, out KeyValueContainer? v))
                return v.Get<T>();

            return defaultValue;
        }

        public static readonly byte[] Zero = new byte[60];

        // Build keyvalues and passthrough to C++ side
        internal unsafe void Build(KeyValuesEntry** entries)
        {
            int arrayOffset = 0;
            foreach(KeyValuePair<string, KeyValueContainer> container in this.KeyValues)
            {
                // Alloc key string
                nint keyPtr = AllocString(container.Key);

                // Alloc KeyValue entry
                nint kvPtr = Marshal.AllocHGlobal(60); // Fixed sizeof
                Marshal.Copy(Zero, 0, kvPtr, 60); // Cleanup memory

                KeyValuesEntry* kvEntry = (KeyValuesEntry*) kvPtr;
                kvEntry->KeyName = keyPtr;
                kvEntry->ValueType = container.Value.GetContainerType();

                switch (container.Value.GetContainerType())
                {
                    case KeyValuesType.TYPE_BOOL:
                        kvEntry->Value.BoolValue = container.Value.Get<bool>();
                        break;

                    case KeyValuesType.TYPE_INT:
                        kvEntry->Value.IntValue = container.Value.Get<int>();
                        break;

                    case KeyValuesType.TYPE_UINT:
                    case KeyValuesType.TYPE_STRING_TOKEN:
                    case KeyValuesType.TYPE_EHANDLE:
                        kvEntry->Value.UIntValue = container.Value.Get<uint>();
                        break;

                    case KeyValuesType.TYPE_INT64:
                        kvEntry->Value.Int64Value = container.Value.Get<long>();
                        break;

                    case KeyValuesType.TYPE_UINT64:
                        kvEntry->Value.UInt64Value = container.Value.Get<ulong>();
                        break;

                    case KeyValuesType.TYPE_FLOAT:
                        kvEntry->Value.FloatValue = container.Value.Get<float>();
                        break;

                    case KeyValuesType.TYPE_DOUBLE:
                        kvEntry->Value.DoubleValue = container.Value.Get<double>();
                        break;

                    case KeyValuesType.TYPE_STRING:
                        // Alloc value string
                        nint strPtr = AllocString(container.Value.Get<string>());
                        kvEntry->Value.PointerValue = strPtr;
                        break;

                    case KeyValuesType.TYPE_POINTER:
                        kvEntry->Value.PointerValue = container.Value.Get<nint>();
                        break;

                    case KeyValuesType.TYPE_COLOR:
                        Color color = container.Value.Get<Color>();
                        kvEntry->Value.ColorValue.R = color.R;
                        kvEntry->Value.ColorValue.G = color.G;
                        kvEntry->Value.ColorValue.B = color.B;
                        kvEntry->Value.ColorValue.A = color.A;
                        break;

                    case KeyValuesType.TYPE_VECTOR:
                        Vector3 vec = container.Value.Get<Vector3>();
                        kvEntry->Value.Vector3Value.X = vec.X;
                        kvEntry->Value.Vector3Value.Y = vec.Y;
                        kvEntry->Value.Vector3Value.Z = vec.Z;
                        break;

                    case KeyValuesType.TYPE_VECTOR2D:
                        Vector2 vec2 = container.Value.Get<Vector2>();
                        kvEntry->Value.Vector2Value.X = vec2.X;
                        kvEntry->Value.Vector2Value.Y = vec2.Y;
                        break;

                    case KeyValuesType.TYPE_VECTOR4D:
                    case KeyValuesType.TYPE_QUATERNION:
                        Vector4 vec4 = container.Value.Get<Vector4>();
                        kvEntry->Value.Vector4Value.X = vec4.X;
                        kvEntry->Value.Vector4Value.Y = vec4.Y;
                        kvEntry->Value.Vector4Value.Z = vec4.Z;
                        kvEntry->Value.Vector4Value.W = vec4.W;
                        break;

                    case KeyValuesType.TYPE_QANGLE:
                        EKVAngle angle = container.Value.Get<EKVAngle>();
                        kvEntry->Value.AngleValue.Pitch = angle.Pitch;
                        kvEntry->Value.AngleValue.Yaw = angle.Yaw;
                        kvEntry->Value.AngleValue.Roll = angle.Roll;
                        break;

                    case KeyValuesType.TYPE_MATRIX3X4:
                        Matrix3x4 matrix = container.Value.Get<Matrix3x4>();
                        kvEntry->Value.Matrix3x4Value.M11 = matrix.M11;
                        kvEntry->Value.Matrix3x4Value.M12 = matrix.M12;
                        kvEntry->Value.Matrix3x4Value.M13 = matrix.M13;
                        kvEntry->Value.Matrix3x4Value.M14 = matrix.M14;

                        kvEntry->Value.Matrix3x4Value.M21 = matrix.M21;
                        kvEntry->Value.Matrix3x4Value.M22 = matrix.M22;
                        kvEntry->Value.Matrix3x4Value.M23 = matrix.M23;
                        kvEntry->Value.Matrix3x4Value.M24 = matrix.M24;

                        kvEntry->Value.Matrix3x4Value.M31 = matrix.M31;
                        kvEntry->Value.Matrix3x4Value.M32 = matrix.M32;
                        kvEntry->Value.Matrix3x4Value.M33 = matrix.M33;
                        kvEntry->Value.Matrix3x4Value.M34 = matrix.M34;
                        break;
                }

                this.KeyValuePtrs.Add(kvPtr);

                entries[arrayOffset] = kvEntry;
                arrayOffset++;
            }
        }

        internal void Free()
        {
            this.KeyValuePtrs.ForEach(Marshal.FreeHGlobal);
            this.StringPtrs.ForEach(Marshal.FreeHGlobal);
        }

        internal nint AllocString(string value)
        {
            nint strPtr = Marshal.AllocHGlobal(value.Length + 1);
            Marshal.Copy(Encoding.UTF8.GetBytes(value), 0, strPtr, value.Length);
            Marshal.WriteByte(strPtr, value.Length, 0);
            this.StringPtrs.Add(strPtr);
            return strPtr;
        }
    }

    internal class KeyValueContainer
    {
        private KeyValuesType type;
        private object value;

        public KeyValueContainer(KeyValuesType type, object value)
        {
            this.type = type;
            this.value = value;
        }

        public KeyValuesType GetContainerType() => type;
        public T Get<T>() => (T)value;
#pragma warning disable 8601 // No it will not be null so shut up
        public void Set<T>(T val) => value = val;
#pragma warning restore
    }

    internal enum KeyValuesType : uint
    {
        TYPE_BOOL,
        TYPE_INT,
        TYPE_UINT,
        TYPE_INT64,
        TYPE_UINT64,
        TYPE_FLOAT,
        TYPE_DOUBLE,
        TYPE_STRING,
        TYPE_POINTER,
        TYPE_STRING_TOKEN,
        TYPE_EHANDLE,
        TYPE_COLOR,
        TYPE_VECTOR,
        TYPE_VECTOR2D,
        TYPE_VECTOR4D,
        TYPE_QUATERNION,
        TYPE_QANGLE,
        TYPE_MATRIX3X4
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct KeyValuesEntry
    {
        public nint KeyName;
        public KeyValuesType ValueType;
        public KeyValuesEntryUnion Value; // This will act like union on C++ side
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 48)]
    internal struct KeyValuesEntryUnion
    {
        [FieldOffset(0)]
        public bool BoolValue;

        [FieldOffset(0)]
        public int IntValue;

        [FieldOffset(0)]
        public uint UIntValue;

        [FieldOffset(0)]
        public long Int64Value;

        [FieldOffset(0)]
        public ulong UInt64Value;

        [FieldOffset(0)]
        public float FloatValue;

        [FieldOffset(0)]
        public double DoubleValue;

        [FieldOffset(0)]
        public nint PointerValue;

        [FieldOffset(0)]
        public EKVColor ColorValue;

        [FieldOffset(0)]
        public Vector2 Vector2Value;

        [FieldOffset(0)]
        public Vector3 Vector3Value;

        [FieldOffset(0)]
        public Vector4 Vector4Value;

        [FieldOffset(0)]
        public EKVAngle AngleValue;

        [FieldOffset(0)]
        public Matrix3x4 Matrix3x4Value;
    }

    internal struct EKVColor
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public EKVColor(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }

    public struct EKVAngle
    {
        public float Pitch;
        public float Yaw;
        public float Roll;

        public EKVAngle(float pitch, float yaw, float roll)
        {
            Pitch = pitch;
            Yaw = yaw;
            Roll = roll;
        }
    }
}
