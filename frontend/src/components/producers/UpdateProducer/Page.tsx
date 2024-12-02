import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router";
import { producerApi } from "../../../services/api";
import { getBase64 } from "../../../services/file";
import type { IProducer } from "../../../types";
import type { IUpdateProducerDTO } from "../../../types/dtos";
import { AxiosError } from "axios";
import { ProducerBasicInfo } from "./ProducerBasicInfo";
import { ImageSection } from "./ImageSection";

export default function UpdateProducerPage() {
  const [producer, setProducer] = useState<
    Partial<IProducer & { imageFile: File | null }>
  >({});
  const navigate = useNavigate();
  const { producerId } = useParams();

  useEffect(() => {
    const fetchProducer = async () => {
      if (!producerId) return;
      try {
        const data = await producerApi.get(producerId);
        setProducer(data);
      } catch (error) {
        console.error("Failed to fetch producer:", error);
        navigate({
          search: "?message=Failed to fetch producer&messageType=danger",
        });
      }
    };

    fetchProducer();
  }, [producerId, navigate]);

  const handleProducerChange = (
    updates: Partial<IProducer & { imageFile: File | null }>
  ) => {
    setProducer((prev) => ({ ...prev, ...updates }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!producerId) return;

    if (!producer.name) {
      alert("Please fill in all required fields. Name is required.");
      return;
    }
    if (!producer.description) {
      alert("Please fill in all required fields. Description is required.");
      return;
    }

    let updateProducerDTO: IUpdateProducerDTO = {
      producerId: parseInt(producerId),
      name: producer.name,
      description: producer.description,
    };

    if (producer.imageFile) {
      await getBase64(producer.imageFile, (result) => {
        const extension =
          "." +
            producer.imageFile?.name.split(".")[
              producer.imageFile?.name.split(".").length - 1
            ] || "";

        updateProducerDTO = {
          ...updateProducerDTO,
          imageBase64: result.split(",")[1],
          imageFileExtension: extension,
        };
      });
    }

    try {
      await producerApi.update(producerId, updateProducerDTO);
      navigate({
        pathname: "/producers/my",
        search: "?message=Producer updated successfully&messageType=success",
      });
    } catch (error) {
      console.error("Failed to update producer:", error);
      if (error instanceof AxiosError) {
        navigate({
          search: `?message=${error.response?.data}&messageType=danger`,
        });
      } else {
        navigate({
          search: "?message=Failed to update producer&messageType=danger",
        });
      }
    }
  };

  if (!producer) return <div>Loading...</div>;

  return (
    <div className="container my-4">
      <div className="row">
        <div className="col-12">
          <h1 className="text-center mb-4">Edit Producer</h1>

          <form onSubmit={handleSubmit}>
            <ProducerBasicInfo
              producer={producer}
              onProducerChange={handleProducerChange}
            />

            <ImageSection
              producer={producer}
              onProducerChange={handleProducerChange}
            />

            <div className="form-group mt-4">
              <button type="submit" className="btn btn-success me-2">
                Save Changes
              </button>
              <a href="/producers/my" className="btn btn-secondary">
                Cancel
              </a>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
