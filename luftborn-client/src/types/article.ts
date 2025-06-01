export interface Translation {
    id: number;
    title: string;
    content: string;
    languageKey: string;
    createdAt: string;
    updatedAt: string | null;
}

export interface Article {
    id: number;
    writerId: string;
    writerName: string;
    writerEmail: string;
    createdAt: string;
    updatedAt: string | null;
    isPublished: boolean;
    currentTranslation: Translation | null;
    allTranslations: Translation[];
}

export interface CreateTranslation {
    title: string;
    content: string;
    languageKey: string;
}

export interface CreateArticle {
    isPublished: boolean;
    translations: CreateTranslation[];
}

export interface UpdateTranslation {
    id?: number;
    title: string;
    content: string;
    languageKey: string;
}

export interface UpdateArticle {
    isPublished: boolean;
    translations: UpdateTranslation[];
} 