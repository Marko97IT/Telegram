// Copyright (c) 2024 Marco Concas. All rights reserved.
// Licensed under the Apache License.

namespace Telegram.Types
{
    public class User
    {
        public long Id { get; set; }
        public bool IsBot { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? LanguageCode { get; set; }
        public bool? IsPremium { get; set; }
        public bool? AddedToAttachmentMenu { get; set; }
        public bool? CanJoinGroups { get; set; }
        public bool? CanReadAllGroupMessages { get; set; }
        public bool? SupportsInlineQueries { get; set; }
    }
}