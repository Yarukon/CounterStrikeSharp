// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: network_connection.proto

#ifndef GOOGLE_PROTOBUF_INCLUDED_network_5fconnection_2eproto
#define GOOGLE_PROTOBUF_INCLUDED_network_5fconnection_2eproto

#include <limits>
#include <string>

#include <google/protobuf/port_def.inc>
#if PROTOBUF_VERSION < 3021000
#error This file was generated by a newer version of protoc which is
#error incompatible with your Protocol Buffer headers. Please update
#error your headers.
#endif
#if 3021008 < PROTOBUF_MIN_PROTOC_VERSION
#error This file was generated by an older version of protoc which is
#error incompatible with your Protocol Buffer headers. Please
#error regenerate this file with a newer version of protoc.
#endif

#include <google/protobuf/port_undef.inc>
#include <google/protobuf/io/coded_stream.h>
#include <google/protobuf/arena.h>
#include <google/protobuf/arenastring.h>
#include <google/protobuf/generated_message_util.h>
#include <google/protobuf/metadata_lite.h>
#include <google/protobuf/generated_message_reflection.h>
#include <google/protobuf/repeated_field.h>  // IWYU pragma: export
#include <google/protobuf/extension_set.h>  // IWYU pragma: export
#include <google/protobuf/generated_enum_reflection.h>
#include <google/protobuf/descriptor.pb.h>
// @@protoc_insertion_point(includes)
#include <google/protobuf/port_def.inc>
#define PROTOBUF_INTERNAL_EXPORT_network_5fconnection_2eproto
PROTOBUF_NAMESPACE_OPEN
namespace internal {
class AnyMetadata;
}  // namespace internal
PROTOBUF_NAMESPACE_CLOSE

// Internal implementation detail -- do not use these members.
struct TableStruct_network_5fconnection_2eproto {
  static const uint32_t offsets[];
};
extern const ::PROTOBUF_NAMESPACE_ID::internal::DescriptorTable descriptor_table_network_5fconnection_2eproto;
PROTOBUF_NAMESPACE_OPEN
PROTOBUF_NAMESPACE_CLOSE

enum ENetworkDisconnectionReason : int {
  NETWORK_DISCONNECT_INVALID = 0,
  NETWORK_DISCONNECT_SHUTDOWN = 1,
  NETWORK_DISCONNECT_DISCONNECT_BY_USER = 2,
  NETWORK_DISCONNECT_DISCONNECT_BY_SERVER = 3,
  NETWORK_DISCONNECT_LOST = 4,
  NETWORK_DISCONNECT_OVERFLOW = 5,
  NETWORK_DISCONNECT_STEAM_BANNED = 6,
  NETWORK_DISCONNECT_STEAM_INUSE = 7,
  NETWORK_DISCONNECT_STEAM_TICKET = 8,
  NETWORK_DISCONNECT_STEAM_LOGON = 9,
  NETWORK_DISCONNECT_STEAM_AUTHCANCELLED = 10,
  NETWORK_DISCONNECT_STEAM_AUTHALREADYUSED = 11,
  NETWORK_DISCONNECT_STEAM_AUTHINVALID = 12,
  NETWORK_DISCONNECT_STEAM_VACBANSTATE = 13,
  NETWORK_DISCONNECT_STEAM_LOGGED_IN_ELSEWHERE = 14,
  NETWORK_DISCONNECT_STEAM_VAC_CHECK_TIMEDOUT = 15,
  NETWORK_DISCONNECT_STEAM_DROPPED = 16,
  NETWORK_DISCONNECT_STEAM_OWNERSHIP = 17,
  NETWORK_DISCONNECT_SERVERINFO_OVERFLOW = 18,
  NETWORK_DISCONNECT_TICKMSG_OVERFLOW = 19,
  NETWORK_DISCONNECT_STRINGTABLEMSG_OVERFLOW = 20,
  NETWORK_DISCONNECT_DELTAENTMSG_OVERFLOW = 21,
  NETWORK_DISCONNECT_TEMPENTMSG_OVERFLOW = 22,
  NETWORK_DISCONNECT_SOUNDSMSG_OVERFLOW = 23,
  NETWORK_DISCONNECT_SNAPSHOTOVERFLOW = 24,
  NETWORK_DISCONNECT_SNAPSHOTERROR = 25,
  NETWORK_DISCONNECT_RELIABLEOVERFLOW = 26,
  NETWORK_DISCONNECT_BADDELTATICK = 27,
  NETWORK_DISCONNECT_NOMORESPLITS = 28,
  NETWORK_DISCONNECT_TIMEDOUT = 29,
  NETWORK_DISCONNECT_DISCONNECTED = 30,
  NETWORK_DISCONNECT_LEAVINGSPLIT = 31,
  NETWORK_DISCONNECT_DIFFERENTCLASSTABLES = 32,
  NETWORK_DISCONNECT_BADRELAYPASSWORD = 33,
  NETWORK_DISCONNECT_BADSPECTATORPASSWORD = 34,
  NETWORK_DISCONNECT_HLTVRESTRICTED = 35,
  NETWORK_DISCONNECT_NOSPECTATORS = 36,
  NETWORK_DISCONNECT_HLTVUNAVAILABLE = 37,
  NETWORK_DISCONNECT_HLTVSTOP = 38,
  NETWORK_DISCONNECT_KICKED = 39,
  NETWORK_DISCONNECT_BANADDED = 40,
  NETWORK_DISCONNECT_KICKBANADDED = 41,
  NETWORK_DISCONNECT_HLTVDIRECT = 42,
  NETWORK_DISCONNECT_PURESERVER_CLIENTEXTRA = 43,
  NETWORK_DISCONNECT_PURESERVER_MISMATCH = 44,
  NETWORK_DISCONNECT_USERCMD = 45,
  NETWORK_DISCONNECT_REJECTED_BY_GAME = 46,
  NETWORK_DISCONNECT_MESSAGE_PARSE_ERROR = 47,
  NETWORK_DISCONNECT_INVALID_MESSAGE_ERROR = 48,
  NETWORK_DISCONNECT_BAD_SERVER_PASSWORD = 49,
  NETWORK_DISCONNECT_DIRECT_CONNECT_RESERVATION = 50,
  NETWORK_DISCONNECT_CONNECTION_FAILURE = 51,
  NETWORK_DISCONNECT_NO_PEER_GROUP_HANDLERS = 52,
  NETWORK_DISCONNECT_RECONNECTION = 53,
  NETWORK_DISCONNECT_LOOPSHUTDOWN = 54,
  NETWORK_DISCONNECT_LOOPDEACTIVATE = 55,
  NETWORK_DISCONNECT_HOST_ENDGAME = 56,
  NETWORK_DISCONNECT_LOOP_LEVELLOAD_ACTIVATE = 57,
  NETWORK_DISCONNECT_CREATE_SERVER_FAILED = 58,
  NETWORK_DISCONNECT_EXITING = 59,
  NETWORK_DISCONNECT_REQUEST_HOSTSTATE_IDLE = 60,
  NETWORK_DISCONNECT_REQUEST_HOSTSTATE_HLTVRELAY = 61,
  NETWORK_DISCONNECT_CLIENT_CONSISTENCY_FAIL = 62,
  NETWORK_DISCONNECT_CLIENT_UNABLE_TO_CRC_MAP = 63,
  NETWORK_DISCONNECT_CLIENT_NO_MAP = 64,
  NETWORK_DISCONNECT_CLIENT_DIFFERENT_MAP = 65,
  NETWORK_DISCONNECT_SERVER_REQUIRES_STEAM = 66,
  NETWORK_DISCONNECT_STEAM_DENY_MISC = 67,
  NETWORK_DISCONNECT_STEAM_DENY_BAD_ANTI_CHEAT = 68,
  NETWORK_DISCONNECT_SERVER_SHUTDOWN = 69,
  NETWORK_DISCONNECT_REPLAY_INCOMPATIBLE = 71,
  NETWORK_DISCONNECT_CONNECT_REQUEST_TIMEDOUT = 72,
  NETWORK_DISCONNECT_SERVER_INCOMPATIBLE = 73,
  NETWORK_DISCONNECT_LOCALPROBLEM_MANYRELAYS = 74,
  NETWORK_DISCONNECT_LOCALPROBLEM_HOSTEDSERVERPRIMARYRELAY = 75,
  NETWORK_DISCONNECT_LOCALPROBLEM_NETWORKCONFIG = 76,
  NETWORK_DISCONNECT_LOCALPROBLEM_OTHER = 77,
  NETWORK_DISCONNECT_REMOTE_TIMEOUT = 79,
  NETWORK_DISCONNECT_REMOTE_TIMEOUT_CONNECTING = 80,
  NETWORK_DISCONNECT_REMOTE_OTHER = 81,
  NETWORK_DISCONNECT_REMOTE_BADCRYPT = 82,
  NETWORK_DISCONNECT_REMOTE_CERTNOTTRUSTED = 83,
  NETWORK_DISCONNECT_UNUSUAL = 84,
  NETWORK_DISCONNECT_INTERNAL_ERROR = 85,
  NETWORK_DISCONNECT_REJECT_BADCHALLENGE = 128,
  NETWORK_DISCONNECT_REJECT_NOLOBBY = 129,
  NETWORK_DISCONNECT_REJECT_BACKGROUND_MAP = 130,
  NETWORK_DISCONNECT_REJECT_SINGLE_PLAYER = 131,
  NETWORK_DISCONNECT_REJECT_HIDDEN_GAME = 132,
  NETWORK_DISCONNECT_REJECT_LANRESTRICT = 133,
  NETWORK_DISCONNECT_REJECT_BADPASSWORD = 134,
  NETWORK_DISCONNECT_REJECT_SERVERFULL = 135,
  NETWORK_DISCONNECT_REJECT_INVALIDRESERVATION = 136,
  NETWORK_DISCONNECT_REJECT_FAILEDCHANNEL = 137,
  NETWORK_DISCONNECT_REJECT_CONNECT_FROM_LOBBY = 138,
  NETWORK_DISCONNECT_REJECT_RESERVED_FOR_LOBBY = 139,
  NETWORK_DISCONNECT_REJECT_INVALIDKEYLENGTH = 140,
  NETWORK_DISCONNECT_REJECT_OLDPROTOCOL = 141,
  NETWORK_DISCONNECT_REJECT_NEWPROTOCOL = 142,
  NETWORK_DISCONNECT_REJECT_INVALIDCONNECTION = 143,
  NETWORK_DISCONNECT_REJECT_INVALIDCERTLEN = 144,
  NETWORK_DISCONNECT_REJECT_INVALIDSTEAMCERTLEN = 145,
  NETWORK_DISCONNECT_REJECT_STEAM = 146,
  NETWORK_DISCONNECT_REJECT_SERVERAUTHDISABLED = 147,
  NETWORK_DISCONNECT_REJECT_SERVERCDKEYAUTHINVALID = 148,
  NETWORK_DISCONNECT_REJECT_BANNED = 149,
  NETWORK_DISCONNECT_KICKED_TEAMKILLING = 150,
  NETWORK_DISCONNECT_KICKED_TK_START = 151,
  NETWORK_DISCONNECT_KICKED_UNTRUSTEDACCOUNT = 152,
  NETWORK_DISCONNECT_KICKED_CONVICTEDACCOUNT = 153,
  NETWORK_DISCONNECT_KICKED_COMPETITIVECOOLDOWN = 154,
  NETWORK_DISCONNECT_KICKED_TEAMHURTING = 155,
  NETWORK_DISCONNECT_KICKED_HOSTAGEKILLING = 156,
  NETWORK_DISCONNECT_KICKED_VOTEDOFF = 157,
  NETWORK_DISCONNECT_KICKED_IDLE = 158,
  NETWORK_DISCONNECT_KICKED_SUICIDE = 159,
  NETWORK_DISCONNECT_KICKED_NOSTEAMLOGIN = 160,
  NETWORK_DISCONNECT_KICKED_NOSTEAMTICKET = 161
};
bool ENetworkDisconnectionReason_IsValid(int value);
constexpr ENetworkDisconnectionReason ENetworkDisconnectionReason_MIN = NETWORK_DISCONNECT_INVALID;
constexpr ENetworkDisconnectionReason ENetworkDisconnectionReason_MAX = NETWORK_DISCONNECT_KICKED_NOSTEAMTICKET;
constexpr int ENetworkDisconnectionReason_ARRAYSIZE = ENetworkDisconnectionReason_MAX + 1;

const ::PROTOBUF_NAMESPACE_ID::EnumDescriptor* ENetworkDisconnectionReason_descriptor();
template<typename T>
inline const std::string& ENetworkDisconnectionReason_Name(T enum_t_value) {
  static_assert(::std::is_same<T, ENetworkDisconnectionReason>::value ||
    ::std::is_integral<T>::value,
    "Incorrect type passed to function ENetworkDisconnectionReason_Name.");
  return ::PROTOBUF_NAMESPACE_ID::internal::NameOfEnum(
    ENetworkDisconnectionReason_descriptor(), enum_t_value);
}
inline bool ENetworkDisconnectionReason_Parse(
    ::PROTOBUF_NAMESPACE_ID::ConstStringParam name, ENetworkDisconnectionReason* value) {
  return ::PROTOBUF_NAMESPACE_ID::internal::ParseNamedEnum<ENetworkDisconnectionReason>(
    ENetworkDisconnectionReason_descriptor(), name, value);
}
// ===================================================================


// ===================================================================

static const int kNetworkConnectionTokenFieldNumber = 50500;
extern ::PROTOBUF_NAMESPACE_ID::internal::ExtensionIdentifier< ::PROTOBUF_NAMESPACE_ID::EnumValueOptions,
    ::PROTOBUF_NAMESPACE_ID::internal::StringTypeTraits, 9, false >
  network_connection_token;

// ===================================================================

#ifdef __GNUC__
  #pragma GCC diagnostic push
  #pragma GCC diagnostic ignored "-Wstrict-aliasing"
#endif  // __GNUC__
#ifdef __GNUC__
  #pragma GCC diagnostic pop
#endif  // __GNUC__

// @@protoc_insertion_point(namespace_scope)


PROTOBUF_NAMESPACE_OPEN

template <> struct is_proto_enum< ::ENetworkDisconnectionReason> : ::std::true_type {};
template <>
inline const EnumDescriptor* GetEnumDescriptor< ::ENetworkDisconnectionReason>() {
  return ::ENetworkDisconnectionReason_descriptor();
}

PROTOBUF_NAMESPACE_CLOSE

// @@protoc_insertion_point(global_scope)

#include <google/protobuf/port_undef.inc>
#endif  // GOOGLE_PROTOBUF_INCLUDED_GOOGLE_PROTOBUF_INCLUDED_network_5fconnection_2eproto
