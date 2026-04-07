export interface HateoasLink {
	rel: string;
	href: string;
}

export function addBookLinks(book: { id: number; authorId: number; publishingCompanyId: number }): HateoasLink[] {
	return [
		{ rel: "self", href: `/books/${book.id}` },
		{ rel: "author", href: `/authors/${book.authorId}` },
		{ rel: "publisher", href: `/publishers/${book.publishingCompanyId}` },
		{ rel: "collection", href: "/books" }
	];
}

export function addAuthorLinks(author: { id: number }): HateoasLink[] {
	return [
		{ rel: "self", href: `/authors/${author.id}` },
		{ rel: "collection", href: "/authors" }
	];
}

export function addPublisherLinks(publisher: { id: number }): HateoasLink[] {
	return [
		{ rel: "self", href: `/publishers/${publisher.id}` },
		{ rel: "collection", href: "/publishers" }
	];
}

export function withLinks<T>(data: T, links: HateoasLink[]): T & { _links: HateoasLink[] } {
	return { ...data, _links: links };
}

export function withLinksArray<T>(items: T[], linkGenerator: (item: T) => HateoasLink[]): (T & { _links: HateoasLink[] })[] {
	return items.map((item) => withLinks(item, linkGenerator(item)));
}
