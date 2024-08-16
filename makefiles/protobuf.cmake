set(PROTO_TARGETS
	${SOURCESDK_DIR}/common/network_connection.proto
	${SOURCESDK_DIR}/common/networkbasetypes.proto
    ${SOURCESDK_DIR}/common/netmessages.proto
)

if (UNIX)
	set(PROTOC_EXECUTABLE ${SOURCESDK_DIR}/devtools/bin/linux/protoc)
elseif(WIN32)
	set(PROTOC_EXECUTABLE ${SOURCESDK_DIR}/devtools/bin/protoc.exe)
endif()

foreach(PROTO_TARGET ${PROTO_TARGETS})
	get_filename_component(PROTO_FILENAME ${PROTO_TARGET} NAME_WLE)
	list(APPEND PROTO_OUTPUT ${PROTO_FILENAME}.pb.cc)
	list(APPEND PROTO_INPUT ${PROTO_FILENAME}.proto)
	get_filename_component(PROTO_PATH ${PROTO_TARGET} DIRECTORY)
	list(APPEND PROTO_PATHS "--proto_path=${PROTO_PATH}")
endforeach()

list(REMOVE_DUPLICATES PROTO_PATHS)
list(TRANSFORM PROTO_OUTPUT PREPEND ${CMAKE_CURRENT_BINARY_DIR}/protobufs/)
file(MAKE_DIRECTORY ${CMAKE_CURRENT_BINARY_DIR}/protobufs)

add_custom_command(
	OUTPUT ${PROTO_OUTPUT}
	COMMAND "${PROTOC_EXECUTABLE}" --proto_path=${SOURCESDK_DIR}/thirdparty/protobuf-3.21.8/src ${PROTO_PATHS} --cpp_out=${CMAKE_CURRENT_BINARY_DIR}/protobufs ${PROTO_INPUT}
	COMMENT "Generating protobuf file"
)
