#region

using Darkages.Network.ClientFormats;

#endregion

namespace Darkages.Network.Login
{
    public class LoginClient : NetworkClient
    {
        public ClientFormat02 CreateInfo { get; set; }

        /// <summary>게임 표로 만들기를 시작했으면 그 카카오 회원번호 — 0x04 가 새 캐릭터의 주인으로 묶는다.</summary>
        public string CreateKakaoId { get; set; }
    }
}