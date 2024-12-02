import { IProduct } from "../../../types";
import { PRODUCT_CATEGORIES } from "../../shared/constants";

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
    <>
      <div className="form-group mb-3">
        <label className="form-label">
          Category<span className="text-danger">*</span>
        </label>
        <select
          className="form-select"
          required
          value={product.category || ""}
          onChange={(e) => onProductChange({ category: e.target.value })}
        >
          <option value="">-- Select a Category --</option>
          {PRODUCT_CATEGORIES.map((c) => (
            <option key={c} value={c}>
              {c}
            </option>
          ))}
        </select>
      </div>

      <div className="form-group mb-3">
        <label className="form-label">
          Product Image<span className="text-danger">*</span>
        </label>
        <input
          type="file"
          className="form-control"
          required
          accept="image/*"
          onChange={(e) => {
            onProductChange({ imageFile: e.target.files?.[0] });
          }}
        />
      </div>
    </>
  );
}
