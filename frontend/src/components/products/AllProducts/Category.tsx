import type { ICategory } from "../../../types";

export function Category({ category }: { category: ICategory }) {
  return (
    <a
      href={`/?category=${category.name}`}
      className="col category-link text-center py-4 block bg-white mb-4 shadow-sm"
    >
      <img src={category.imageUrl} className="icon" alt="Fruits" />
      <h5>{category.name}</h5>
    </a>
  );
}
