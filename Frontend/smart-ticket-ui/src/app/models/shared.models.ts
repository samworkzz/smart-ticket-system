export interface PagedRequest {
    pageNumber: number;
    pageSize: number;
    sortBy?: string;
    sortDescending?: boolean;
}

export interface PagedResponse<T> {
    items: T[];
    pageNumber: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
}
