package com.laboratory6;

import org.apache.commons.lang3.tuple.Pair;

import javax.imageio.ImageIO;
import java.awt.Color;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.List;
import java.util.concurrent.ForkJoinPool;
import java.util.stream.Collectors;
import java.util.stream.Stream;

public class Main {
    public static void main(String[] args) {
        String inputDirectory;
        String outputDirectory;

        System.out.println("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
        System.out.println(">> [INFO] Default directories:");
        System.out.println(">> [INFO]     >> src/main/resources/input (source),");
        System.out.println(">> [INFO]     >> src/main/resources/output (destination).");
        System.out.println("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");

        if (args.length < 2) {
            inputDirectory = "src/main/resources/input";
            outputDirectory = "src/main/resources/output";
        } else {
            inputDirectory = args[0];
            outputDirectory = args[1];
        }

        int[] threadCounts = {1, 2, 4, 8};

        for (int threadCount : threadCounts) {
            System.out.printf(">> [INFO] Thread count: %d%n", threadCount);
            long startTime = System.currentTimeMillis();

            ForkJoinPool forkJoinPool = new ForkJoinPool(threadCount);

            try {
                forkJoinPool.submit(() -> {
                    try {
                        List<Pair<String, BufferedImage>> images = processImages(inputDirectory);
                        saveImages(images, outputDirectory);
                        System.out.println(">> [INFO] Processing completed successfully."); 
                    } catch (IOException e) {
                        System.err.printf(">> [INFO] Error processing images: %s%n", e.getMessage());
                    }
                }).get();
            } catch (Exception e) {
                e.printStackTrace();
            }

            long endTime = System.currentTimeMillis();
            System.out.printf(">> [INFO] Time taken: %d ms%n", (endTime - startTime));
            System.out.println("=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
        }
    }

    private static List<Pair<String, BufferedImage>> processImages(String inputDirectory) throws IOException {
        try (Stream<Path> paths = Files.list(Path.of(inputDirectory))) {
            return paths.parallel()
                    .map(path -> {
                        try {
                            BufferedImage image = ImageIO.read(path.toFile());
                            return Pair.of(path.getFileName().toString(), image);
                        } catch (IOException e) {
                            System.err.printf(">> [INFO] Error reading image: %s%n", e.getMessage());
                            return null;
                        }
                    })
                    .filter(pair -> pair != null)
                    .collect(Collectors.toList());
        }
    }

    private static void saveImages(List<Pair<String, BufferedImage>> images, String outputDirectory) {
        images.parallelStream().forEach(pair -> {
            try {
                String name = pair.getLeft();
                BufferedImage image = pair.getRight();
                BufferedImage transformedImage = transformImage(image);
                File outputFile = new File(outputDirectory + File.separator + name);
                ImageIO.write(transformedImage, "jpg", outputFile);
            } catch (IOException e) {
                System.err.printf(">> [INFO] Error saving image: %s%n", e.getMessage());
            }
        });
    }

    private static BufferedImage transformImage(BufferedImage image) {
        BufferedImage transformedImage = new BufferedImage(image.getWidth(), image.getHeight(), BufferedImage.TYPE_BYTE_GRAY);
        for (int i = 0; i < image.getWidth(); i++) {
            for (int j = 0; j < image.getHeight(); j++) {
                var color = new Color(image.getRGB(i, j));
                var grayScale = (int) (color.getRed() * 0.299 + color.getGreen() * 0.587 + color.getBlue() * 0.114);
                transformedImage.setRGB(i, j, new Color(grayScale, grayScale, grayScale).getRGB());
            }
        }
        return transformedImage;
    }
}
