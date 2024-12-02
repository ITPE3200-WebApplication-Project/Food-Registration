import { IProduct } from "../../../types";
import { PRODUCT_CATEGORIES } from "../../shared/constants";
import { getImageUrl } from "../../../services/file";

interface ProductCategoryInfoProps {
  product: Partial<IProduct & { imageFile: File | null }>;
  onProductChange: (
    updates: Partial<IProduct & { imageFile: File | null }>
  ) => void;
}

export function ProductCategoryInfo({
  product,
  onProductChange,
}: ProductCategoryInfoProps) {
  return (
    <div className="row">
      <div className="col-md-8">
        <div className="form-group mb-3">
          <label className="form-label">
            Category<span className="text-danger">*</span>
          </label>
          <select
            className="form-select"
            required
            value={product.category}
            onChange={(e) => onProductChange({ category: e.target.value })}
          >
            {PRODUCT_CATEGORIES.map((c) => (
              <option key={c} value={c}>
                {c}
              </option>
            ))}
          </select>
        </div>

        <div className="form-group mb-3">
          <label className="form-label">Change Image</label>
          <input
            type="file"
            className="form-control"
            accept="image/*"
            onChange={(e) =>
              onProductChange({
                imageFile: e.target.files?.[0],
              })
            }
          />
        </div>
      </div>
      <div className="col-md-4">
        {product.imageUrl && (
          <div className="mb-3">
            <label className="form-label">Current Image</label>
            <img
              src={getImageUrl(product.imageUrl)}
              alt="Current Product"
              className="img-fluid rounded"
            />
          </div>
        )}
      </div>
    </div>
  );
}
