using Microsoft.AspNetCore.Http.Features;

namespace FeiniuBus.AspNetCore.Buffering
{
    internal class HttpBufferingFeature : IHttpBufferingFeature
    {
        private readonly BufferingReadStream _buffer;
        private readonly IHttpBufferingFeature _innerFeature;

        public HttpBufferingFeature(BufferingReadStream buffer, IHttpBufferingFeature innerFeature)
        {
            _innerFeature = innerFeature;
            _buffer = buffer;
        }
        
        public void DisableRequestBuffering()
        {
            _buffer.DisableBuffering();
            _innerFeature.DisableRequestBuffering();
        }

        public void DisableResponseBuffering()
        {
            _innerFeature?.DisableResponseBuffering();
        }
    }
}