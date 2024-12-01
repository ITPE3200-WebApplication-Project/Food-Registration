import { useEffect, useState } from "react";
import { IProducer, type IProduct } from "../../../types";
import { PRODUCT_CATEGORIES, NUTRITION_SCORES } from "../../shared/constants";
import { producerApi, productApi } from "../../../services/api";
import { ICreateProductDTO } from "../../../types/dtos";

export default function Create() {
  const [product, setProduct] = useState<
    Partial<IProduct & { imageFile: File | null }>
  >({});
  const [producers, setProducers] = useState<IProducer[]>([]);

  useEffect(() => {
    producerApi.getMyProducers().then(setProducers);
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (product) {
      if (
        !product.name ||
        !product.producer ||
        !product.category ||
        !product.nutritionScore
      ) {
        alert("Please fill in all required fields");
        return;
      }
      console.log(product);
      const createProductDTO: ICreateProductDTO = {
        name: product.name,
        description: product.description,
        category: product.category,
        nutritionScore: product.nutritionScore,
        producerId: product.producer?.producerId || 0,
        imageFile: product.imageFile || null,
        calories: product.calories,
        carbohydrates: product.carbohydrates,
        fat: product.fat,
        protein: product.protein,
      };

      console.log(createProductDTO);
      setProduct(createProductDTO);
      await productApi.create(createProductDTO);

      console.log("Product created");
    }
  };

  return (
    <div className="container my-4">
      <div className="row">
        <div className="col-12">
          <h1 className="text-center mb-4">Create New Product</h1>

          <form onSubmit={handleSubmit}>
            <div className="form-group mb-3">
              <label className="form-label">
                Name<span className="text-danger">*</span>
              </label>
              <input
                className="form-control"
                value={product.name || ""}
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
                value={product.producer?.name || ""}
                onChange={(e) =>
                  setProduct({
                    ...product,
                    producer: producers.find(
                      (p: { name: string }) => p.name === e.target.value
                    ),
                  })
                }
              >
                <option value="">-- Select a Producer --</option>
                {producers?.map((p) => (
                  <option key={p.producerId} value={p.name}>
                    {p.name}
                  </option>
                ))}
              </select>
            </div>

            <div className="form-group mb-3">
              <label className="form-label">Description</label>
              <textarea
                className="form-control"
                value={product.description || ""}
                onChange={(e) =>
                  setProduct({ ...product, description: e.target.value })
                }
              />
            </div>

            <div className="form-group mb-3">
              <label className="form-label">Category</label>
              <select
                className="form-select"
                value={product.category || ""}
                onChange={(e) =>
                  setProduct({ ...product, category: e.target.value })
                }
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
              <label className="form-label">Product Image</label>
              <input
                type="file"
                className="form-control"
                accept="image/*"
                onChange={(e) =>
                  setProduct({ ...product, imageFile: e.target.files?.[0] })
                }
              />
            </div>

            <div className="mb-3">
              <h5>Nutritional Values (per 100g)</h5>
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
                    value={product.calories || ""}
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
                    value={product.protein || ""}
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
                    value={product.carbohydrates || ""}
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
                    value={product.fat || ""}
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
                value={product.nutritionScore || ""}
                onChange={(e) =>
                  setProduct({ ...product, nutritionScore: e.target.value })
                }
              >
                <option value="">-- Select a Nutrition Score --</option>
                {NUTRITION_SCORES.map((score) => (
                  <option key={score} value={score}>
                    {score}
                  </option>
                ))}
              </select>
            </div>

            <div className="form-group mt-4">
              <button type="submit" className="btn btn-success">
                Create Product
              </button>
              <a href="/" className="btn btn-secondary">
                Cancel
              </a>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
