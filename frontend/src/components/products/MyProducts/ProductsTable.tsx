import { IProduct } from "../../../types";

interface ProductsTableProps {
  products: IProduct[];
  onDelete: (id: number, name: string) => void;
}

export function ProductsTable({ products, onDelete }: ProductsTableProps) {
  return (
    <table className="table table-hover table-bordered mt-2">
      <thead className="table-primary">
        <tr>
          <th>Name</th>
          <th>Producer</th>
          <th className="text-center">Nutrition Score</th>
          <th className="text-center">Update</th>
          <th className="text-center">Delete</th>
        </tr>
      </thead>
      <tbody>
        {products.map((product) => (
          <tr key={product.productId}>
            <td>{product.name}</td>
            <td>{product.producer?.name || "N/A"}</td>
            <td className="text-center">
              {product.nutritionScore ? (
                <img
                  src={`/images/nutri-scores/Nutri-score-${product.nutritionScore}.svg.png`}
                  alt={`Nutrition Score ${product.nutritionScore}`}
                  className="nutrition-score-table"
                />
              ) : (
                <span>N/A</span>
              )}
            </td>
            <td className="text-center">
              <a
                href={`/products/${product.productId}/update`}
                className="btn btn-sm btn-warning rounded-pill px-3"
              >
                <i className="bi bi-pencil-square"></i> Update
              </a>
            </td>
            <td className="text-center">
              <button
                className="btn btn-danger btn-sm rounded-pill px-3"
                onClick={() => onDelete(product.productId, product.name)}
              >
                <i className="bi bi-trash"></i> Delete
              </button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
