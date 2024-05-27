
struct SoundEventParamFloat
{
public:
    SoundEventParamFloat(float value)
    {
        m_nOpVarType = 1;
        m_nFieldDataSize = 8;
        m_nAllocSize = 4;
        m_nDataSize = 4;
        m_flOpValue = value;
    }

    unsigned int m_nOpVarType;
    unsigned int m_nFieldDataSize;
    unsigned int m_nAllocSize;
    unsigned int m_nDataSize;
    float m_flOpValue;

private:
    unsigned int m_padding0;
};
