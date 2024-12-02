import { IProduct } from "../../../types";

interface ProductInfoProps {
  product: IProduct;
}

export function ProductInfo({ product }: ProductInfoProps) {
  return (
    <>
      <div className="mb-4">
        <h5>Description</h5>
        <p className="card-text">{product.description}</p>
      </div>

      <div className="mb-4">
        <h5>Category</h5>
        <p className="card-text">{product.category}</p>
      </div>
    </>
  );
}
