import { useState } from "react";
import type { IProduct } from "../../types";

interface UpdateProductProps {
  product: IProduct;
  producers: Array<{ id: number; name: string }>;
  categories: string[];
  nutritionScores: string[];
  onUpdate: (product: IProduct, file?: File) => Promise<void>;
}

export default function Update({
  product: initialProduct,
  producers,
  categories,
  nutritionScores,
  onUpdate,
}: UpdateProductProps) {
  const [product, setProduct] = useState<IProduct>(initialProduct);
  const [file, setFile] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState(initialProduct.imageUrl);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await onUpdate(product, file || undefined);
  };

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = e.target.files?.[0];
    if (selectedFile) {
      setFile(selectedFile);
      const reader = new FileReader();
      reader.onload = (e) => {
        setImagePreview(e.target?.result as string);
      };
      reader.readAsDataURL(selectedFile);
    }
  };

  return (
    <div className="container my-4">
      <div className="row">
        <div className="col-12">
          <h1 className="text-center mb-4">Update Product</h1>

          <form onSubmit={handleSubmit} encType="multipart/form-data">
            <div className="row">
              <div className="col-md-8">
                <div className="form-group mb-3">
                  <label className="form-label">
                    Name<span className="text-danger">*</span>
                  </label>
                  <input
                    className="form-control"
                    value={product.name}
                    onChange={(e) =>
                      setProduct({ ...product, name: e.target.value })
                    }
                  />
                </div>

                <div className="form-group mb-3">
                  <label className="form-label">
                    Producer<span className="text-danger">*</span>
                  </label>
                  <select
                    className="form-select"
                    value={product.producer}
                    onChange={(e) =>
                      setProduct({ ...product, producer: e.target.value })
                    }
                  >
                    {producers.map((p) => (
                      <option key={p.id} value={p.id}>
                        {p.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div className="form-group mb-3">
                  <label className="form-label">Description</label>
                  <textarea
                    className="form-control"
                    value={product.description}
                    onChange={(e) =>
                      setProduct({ ...product, description: e.target.value })
                    }
                  />
                </div>

                <div className="form-group mb-3">
                  <label className="form-label">Category</label>
                  <select
                    className="form-select"
                    value={product.category}
                    onChange={(e) =>
                      setProduct({ ...product, category: e.target.value })
                    }
                  >
                    {categories.map((c) => (
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
                    onChange={handleImageChange}
                  />
                </div>

                <div className="row">
                  <div className="col-md-3">
                    <div className="form-group mb-3">
                      <label className="form-label">Calories (kcal)</label>
                      <input
                        type="number"
                        className="form-control"
                        min="0"
                        step="0.1"
                        value={product.calories}
                        onChange={(e) =>
                          setProduct({
                            ...product,
                            calories: parseFloat(e.target.value),
                          })
                        }
                      />
                    </div>
                  </div>
                  <div className="col-md-3">
                    <div className="form-group mb-3">
                      <label className="form-label">Protein (g)</label>
                      <input
                        type="number"
                        className="form-control"
                        min="0"
                        step="0.1"
                        value={product.protein}
                        onChange={(e) =>
                          setProduct({
                            ...product,
                            protein: parseFloat(e.target.value),
                          })
                        }
                      />
                    </div>
                  </div>
                  <div className="col-md-3">
                    <div className="form-group mb-3">
                      <label className="form-label">Carbohydrates (g)</label>
                      <input
                        type="number"
                        className="form-control"
                        min="0"
                        step="0.1"
                        value={product.carbohydrates}
                        onChange={(e) =>
                          setProduct({
                            ...product,
                            carbohydrates: parseFloat(e.target.value),
                          })
                        }
                      />
                    </div>
                  </div>
                  <div className="col-md-3">
                    <div className="form-group mb-3">
                      <label className="form-label">Fat (g)</label>
                      <input
                        type="number"
                        className="form-control"
                        min="0"
                        step="0.1"
                        value={product.fat}
                        onChange={(e) =>
                          setProduct({
                            ...product,
                            fat: parseFloat(e.target.value),
                          })
                        }
                      />
                    </div>
                  </div>
                </div>

                <div className="form-group mb-3">
                  <label className="form-label">
                    Nutrition Score<span className="text-danger">*</span>
                  </label>
                  <select
                    className="form-select"
                    value={product.nutritionScore}
                    onChange={(e) =>
                      setProduct({ ...product, nutritionScore: e.target.value })
                    }
                  >
                    {nutritionScores.map((score) => (
                      <option key={score} value={score}>
                        {score}
                      </option>
                    ))}
                  </select>
                </div>

                <div className="form-group mt-4">
                  <button type="submit" className="btn btn-success">
                    Save Changes
                  </button>
                  <a href="/" className="btn btn-secondary ms-2">
                    Cancel
                  </a>
                </div>
              </div>

              <div className="col-md-4">
                {imagePreview && (
                  <div className="mb-3">
                    <label className="form-label">Current Image</label>
                    <img
                      src={imagePreview}
                      alt="Product Preview"
                      className="img-fluid rounded"
                    />
                  </div>
                )}
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
