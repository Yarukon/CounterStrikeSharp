#ifndef ITEM_SELECTION_CRITERIA_H
#define ITEM_SELECTION_CRITERIA_H
#ifdef _WIN32
#pragma once
#endif

#include "platform.h"
#include "utlstring.h"
#include "utlvector.h"

// Maximum string length in item create APIs
const int k_cchCreateItemLen = 64;

// Operators for BAddNewItemCriteria
enum EItemCriteriaOperator
{
    k_EOperator_String_EQ = 0, // Field is string equal to value
    k_EOperator_Not = 1, // Logical not
    k_EOperator_String_Not_EQ = 1, // Field is not string equal to value
    k_EOperator_Float_EQ = 2, // Field as a float is equal to value
    k_EOperator_Float_Not_EQ = 3, // Field as a float is not equal to value
    k_EOperator_Float_LT = 4, // Field as a float is less than value
    k_EOperator_Float_Not_LT = 5, // Field as a float is not less than value
    k_EOperator_Float_LTE = 6, // Field as a float is less than or equal value
    k_EOperator_Float_Not_LTE = 7, // Field as a float is not less than or equal value
    k_EOperator_Float_GT = 8, // Field as a float is greater than value
    k_EOperator_Float_Not_GT = 9, // Field as a float is not greater than value
    k_EOperator_Float_GTE = 10, // Field as a float is greater than or equal value
    k_EOperator_Float_Not_GTE = 11, // Field as a float is not greater than or equal value
    k_EOperator_Subkey_Contains = 12, // Field contains value as a subkey
    k_EOperator_Subkey_Not_Contains = 13, // Field does not contain value as a subkey

    // Must be last
    k_EItemCriteriaOperator_Count = 14,
};

class CEconItemSchema;
class CEconItemDefinition;
class CEconItem;
class CSOItemCriteria;
class CSOItemCriteriaCondition;
class CCondition;

const uint8 k_unItemRarity_Any = 0xF;
const uint8 k_unItemQuality_Any = 0xF;

//-----------------------------------------------------------------------------
// CItemSelectionCriteria
// A class that contains all the conditions a server needs to specify what
// kind of random item they wish to generate.
//-----------------------------------------------------------------------------
class CItemSelectionCriteria
{
  public:
    // Constructors and destructor
    CItemSelectionCriteria()
        : m_bItemLevelSet(false), m_unItemLevel(0), m_bQualitySet(false), m_nItemQuality(k_unItemQuality_Any), m_bRaritySet(false),
          m_nItemRarity(k_unItemRarity_Any), m_unInitialInventory(0), m_unInitialQuantity(1), m_bForcedQualityMatch(false),
          m_bIgnoreEnabledFlag(false), m_bRecentOnly(false), m_bIsLootList(false)
    {
    }
    // True if item level is specified in this criteria
    bool m_bItemLevelSet;
    // The level of the item to generate
    uint32 m_unItemLevel;
    // True if quality is specified in this criteria
    bool m_bQualitySet;
    // The quality of the item to generate
    int32 m_nItemQuality;
    // True if rarity is specified in this criteria
    bool m_bRaritySet;
    // The rarity of the item to generate
    int32 m_nItemRarity;
    // The initial inventory token of the item
    uint32 m_unInitialInventory;
    // The initial quantity of the item
    uint32 m_unInitialQuantity;
    // Enforced explicit quality matching
    bool m_bForcedQualityMatch;
    // Ignoring enabled flag (used when crafting)
    bool m_bIgnoreEnabledFlag;
    // Check Recent flag
    bool m_bRecentOnly;
    // Outputs an item from a loot list
    bool m_bIsLootList;

    // A list of the conditions
    CUtlVector<CCondition*> m_vecConditions;
};

#endif // ITEM_SELECTION_CRITERIA_H
