#define MINIMP3_ONLY_MP3
#define MINIMP3_NO_SIMD
#define MINIMP3_IMPLEMENTATION
#include "minimp3/minimp3_ex.h"

#if _WIN32
#define MINIMP3_API __declspec(dllexport)
#define MINIMP3_CALL __cdecl
#else
#define MINIMP3_API
#define MINIMP3_CALL
#endif

MINIMP3_API mp3dec_ex_t* MINIMP3_CALL mp3i_open_decoder(uint8_t* buffer, uint64_t buffer_length)
{
    mp3dec_ex_t* dec = (mp3dec_ex_t*)malloc(sizeof(mp3dec_ex_t));
    if(dec == NULL)
    {
        return NULL;
    }

    int res = mp3dec_ex_open_buf(dec, buffer, (size_t)buffer_length, MP3D_SEEK_TO_SAMPLE);
    if(res != 0)
    {
        free(dec);
        dec = NULL;
    }
    return dec;
}

MINIMP3_API void MINIMP3_CALL mp3i_close_decoder(mp3dec_ex_t* decoder)
{
    mp3dec_ex_close(decoder);
    free(decoder);
}

MINIMP3_API uint64_t MINIMP3_CALL mp3i_read_samples(mp3dec_ex_t* decoder, int16_t* buffer, uint64_t buffer_length, int* error)
{
    uint64_t read = mp3dec_ex_read(decoder, buffer, (size_t)buffer_length);
    if(read != buffer_length)
    {
        *error = decoder->last_error;
    }
    return read;
}

MINIMP3_API void MINIMP3_CALL mp3i_stream_info(mp3dec_ex_t* decoder, uint64_t* samples, int32_t* sample_rate, int32_t* channels, int32_t* bitrate)
{
    *samples = decoder->samples;
    *sample_rate = decoder->info.hz;
    *channels = decoder->info.channels;
    *bitrate = decoder->info.bitrate_kbps;
}
